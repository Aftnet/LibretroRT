using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Audio;

namespace LibretroRT.AudioGraphPlayer
{
    public sealed class AudioPlayer : IDisposable, IAudioPlayer
    {
        [ComImport]
        [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        unsafe interface IMemoryBufferByteAccess
        {
            void GetBuffer(out byte* buffer, out uint capacity);
        }

        private const uint NumChannels = 2;
        private const float PlaybackDelaySeconds = 0.1f; //Have some buffer to avoid crackling
        private const float MaxAllowedDelaySeconds = 0.3f; //Limit maximum delay

        private int MinNumSamplesForPlayback = 0;
        private int MaxNumSamplesForTargetDelay = 0;

        private ICore core = null;
        public ICore Core
        {
            get { return core; }
            set
            {
                if (core == value)
                {
                    return;
                }

                if (core != null)
                {
                    Stop();
                    core.TimingChanged -= CoreTimingsChanged;
                    core.RenderAudioFrames -= RenderAudioFrames;
                }

                core = value;
                if (core != null)
                {
                    core.TimingChanged += CoreTimingsChanged;
                    core.RenderAudioFrames += RenderAudioFrames;
                }
            }
        }

        private uint SampleRate { get; set; }

        public bool ShouldDelayNextFrame
        {
            get
            {
                if (SampleRate == 0)
                    return false; //Allow core a chance to init timings by runnning

                lock (SamplesBuffer)
                {
                    return SamplesBuffer.Count >= MaxNumSamplesForTargetDelay;
                }
            }
        }

        private readonly Queue<short> SamplesBuffer = new Queue<short>();

        private bool GraphReconstructionInProgress = false;
        private bool AllowPlaybackControl { get { return !GraphReconstructionInProgress && InputNode != null; } }

        private AudioGraph graph;
        private AudioGraph Graph
        {
            get { return graph; }
            set { graph?.Dispose(); graph = value; }
        }

        private AudioDeviceOutputNode outputNode;
        private AudioDeviceOutputNode OutputNode
        {
            get { return outputNode; }
            set { outputNode?.Dispose(); outputNode = value; }
        }

        private AudioFrameInputNode inputNode;
        private AudioFrameInputNode InputNode
        {
            get { return inputNode; }
            set { inputNode?.Dispose(); inputNode = value; }
        }

        public AudioPlayer()
        {
            SampleRate = 0;
        }

        public void Dispose()
        {
            DisposeGraph();
        }

        private void CoreTimingsChanged(SystemTiming timings)
        {
            uint sampleRate = (uint)timings.SampleRate;
            if (SampleRate == sampleRate || GraphReconstructionInProgress)
                return;

            Stop();
            var operation = ReconstructGraph(sampleRate);
        }

        private void RenderAudioFrames([ReadOnlyArray] short[] samples)
        {
            if (!AllowPlaybackControl)
                return;

            lock (SamplesBuffer)
            {
                foreach (var i in samples)
                {
                    SamplesBuffer.Enqueue(i);
                }

                if (SamplesBuffer.Count >= MinNumSamplesForPlayback)
                {
                    Graph.Start();
                }
            }
        }

        public void Stop()
        {
            if (!AllowPlaybackControl)
                return;

            Graph.Stop();

            lock (SamplesBuffer)
            {
                SamplesBuffer.Clear();
            }
        }

        private async Task ReconstructGraph(uint sampleRate)
        {
            GraphReconstructionInProgress = true;

            SampleRate = sampleRate;
            MinNumSamplesForPlayback = (int)(sampleRate * PlaybackDelaySeconds);
            MaxNumSamplesForTargetDelay = (int)(sampleRate * MaxAllowedDelaySeconds);

            DisposeGraph();
            if (SampleRate == 0) //If invalid sample rate do not create graph but return to allow trying again
            {
                GraphReconstructionInProgress = false;
                return;
            }

            var graphResult = await AudioGraph.CreateAsync(new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.GameMedia));
            if (graphResult.Status != AudioGraphCreationStatus.Success)
            {
                DisposeGraph();
                throw new Exception($"Unable to create audio graph: {graphResult.Status.ToString()}");
            }
            Graph = graphResult.Graph;
            Graph.Stop();

            var outNodeResult = await Graph.CreateDeviceOutputNodeAsync();
            if (outNodeResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                DisposeGraph();
                throw new Exception($"Unable to create device node: {outNodeResult.Status.ToString()}");
            }
            OutputNode = outNodeResult.DeviceOutputNode;

            var nodeProperties = Graph.EncodingProperties;
            nodeProperties.ChannelCount = NumChannels;
            nodeProperties.SampleRate = SampleRate;

            InputNode = Graph.CreateFrameInputNode(nodeProperties);
            InputNode.QuantumStarted += InputNodeQuantumStartedHandler;
            InputNode.AddOutgoingConnection(OutputNode);

            GraphReconstructionInProgress = false;
        }

        private void DisposeGraph()
        {
            InputNode = null;
            OutputNode = null;
            Graph = null;
        }

        private void InputNodeQuantumStartedHandler(AudioFrameInputNode sender, FrameInputNodeQuantumStartedEventArgs args)
        {
            if (args.RequiredSamples < 1)
                return;

            AudioFrame frame = GenerateAudioData(args.RequiredSamples);
            sender.AddFrame(frame);
        }

        unsafe private AudioFrame GenerateAudioData(int requiredSamples)
        {
            // Buffer size is (number of samples) * (size of each sample)
            // We choose to generate single channel (mono) audio. For multi-channel, multiply by number of channels
            uint bufferSizeElements = (uint)requiredSamples * NumChannels;
            uint bufferSizeBytes = bufferSizeElements * sizeof(float);
            AudioFrame frame = new AudioFrame(bufferSizeBytes);

            using (AudioBuffer buffer = frame.LockBuffer(AudioBufferAccessMode.Write))
            using (IMemoryBufferReference reference = buffer.CreateReference())
            {
                byte* dataInBytes;
                uint capacityInBytes;
                float* dataInFloat;

                // Get the buffer from the AudioFrame
                ((IMemoryBufferByteAccess)reference).GetBuffer(out dataInBytes, out capacityInBytes);

                // Cast to float since the data we are generating is float
                dataInFloat = (float*)dataInBytes;

                lock (SamplesBuffer)
                {
                    var numElementsToCopy = Math.Min(bufferSizeElements, SamplesBuffer.Count);
                    for (var i = 0; i < numElementsToCopy; i++)
                    {
                        var converted = (float)SamplesBuffer.Dequeue() / short.MaxValue;
                        dataInFloat[i] = converted;
                    }
                    //Should we not have enough samples in buffer, set the remaing data in audio frame to zeros
                    for (var i = numElementsToCopy; i < bufferSizeElements; i++)
                    {
                        dataInFloat[i] = 0f;
                    }
                }
            }

            return frame;
        }
    }
}
