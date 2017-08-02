using System.Runtime.InteropServices.WindowsRuntime;

namespace LibretroRT.FrontendComponents.Common
{
    public interface IAudioPlayer
    {
        bool ShouldDelayNextFrame { get; }
        void TimingChanged(SystemTiming timings);
        void RenderAudioFrames([ReadOnlyArray] short[] samples);
        void Stop();
    }
}