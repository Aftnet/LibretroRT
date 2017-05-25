using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Media.MediaProperties;

namespace Test
{
    public class AudioPlayer : IDisposable
    {
        [ComImport]
        [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        unsafe interface IMemoryBufferByteAccess
        {
            void GetBuffer(out byte* buffer, out uint capacity);
        }

        private const uint NumChannels = 2;
        private const uint NumBytesPerSample = sizeof(short);
        private const uint NumBitsPerSample = NumBytesPerSample * 8;
        private const uint NullSampleRate = 0;
        private const int BufferingDelayMs = 200;

        public uint SampleRate { get; private set; }

        private bool IsPlaying = false;
        private readonly Queue<short> SamplesBuffer = new Queue<short>();

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
            SampleRate = NullSampleRate;
        }

        public void Dispose()
        {
            DisposeGraph();
        }

        public Task SetSampleRateAsync(uint sampleRate)
        {
            if (SampleRate == sampleRate)
            {
                return Task.FromResult(true);
            }

            Stop();
            return CreateGraph(sampleRate);
        }

        public async void Start()
        {
            IsPlaying = true;
            //Add some delay to fill the buffer with some data and avoid pops
            await (Task.Delay(BufferingDelayMs));
            Graph.Start();
        }

        public void Stop()
        {
            IsPlaying = false;
            Graph.Stop();

            lock(SamplesBuffer)
            {
                SamplesBuffer.Clear();
            }
        }

        public void AddSamples(short[] samples)
        {
            if (!IsPlaying)
                return;

            lock(SamplesBuffer)
            {
                foreach (var i in samples)
                {
                    SamplesBuffer.Enqueue(i);
                }
            }
        }

        private async Task CreateGraph(uint sampleRate)
        {
            DisposeGraph();
            
            var graphResult = await AudioGraph.CreateAsync(new AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.GameMedia)
            {
                EncodingProperties = new AudioEncodingProperties
                {
                    SampleRate = sampleRate,
                    ChannelCount = NumChannels,
                    BitsPerSample = NumBitsPerSample
                }
            });
            if (graphResult.Status != AudioGraphCreationStatus.Success)
            {
                SampleRate = NullSampleRate;
                DisposeGraph();
                throw new Exception($"Unable to create audio graph: {graphResult.Status.ToString()}");
            }

            Graph.Stop();

            var outNodeResult = await Graph.CreateDeviceOutputNodeAsync();
            if (outNodeResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                SampleRate = NullSampleRate;
                DisposeGraph();
                throw new Exception($"Unable to create device node: {outNodeResult.Status.ToString()}");
            }

            InputNode = Graph.CreateFrameInputNode(AudioEncodingProperties.CreatePcm(sampleRate, NumChannels, NumBitsPerSample));
            InputNode.QuantumStarted += InputNodeQuantumStartedHandler;
            InputNode.AddOutgoingConnection(OutputNode);
            SampleRate = sampleRate;
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
            uint bufferSizeBytes = bufferSizeElements * NumBytesPerSample;
            AudioFrame frame = new AudioFrame(bufferSizeBytes);

            using (AudioBuffer buffer = frame.LockBuffer(AudioBufferAccessMode.Write))
            using (IMemoryBufferReference reference = buffer.CreateReference())
            {
                byte* dataInBytes;
                uint capacityInBytes;
                short* dataInShort;

                // Get the buffer from the AudioFrame
                ((IMemoryBufferByteAccess)reference).GetBuffer(out dataInBytes, out capacityInBytes);

                // Cast to float since the data we are generating is float
                dataInShort = (short*)dataInBytes;

                lock (SamplesBuffer)
                {
                    var numElementsToCopy = Math.Min(bufferSizeElements, SamplesBuffer.Count);
                    for (var i = 0; i < numElementsToCopy; i++)
                    {
                        dataInShort[i] = SamplesBuffer.Dequeue();
                    }
                    //Should we not have enough samples in buffer, set the remaing data in audio frame to zeros
                    for (var i = numElementsToCopy; i < bufferSizeElements; i++)
                    {
                        dataInShort[i] = 0;
                    }
                }
            }

            return frame;
        }
    }
}
