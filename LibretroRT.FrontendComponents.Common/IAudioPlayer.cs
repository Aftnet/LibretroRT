using LibRetriX;
using System.IO;

namespace LibretroRT.FrontendComponents.Common
{
    public interface IAudioPlayer
    {
        bool ShouldDelayNextFrame { get; }
        void TimingChanged(SystemTimings timings);
        void RenderAudioFrames(Stream data, ulong numFrames);
        void Stop();
    }
}