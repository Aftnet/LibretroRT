using LibRetriX;
using System.IO;

namespace RetriX.UWP
{
    public interface IAudioPlayer
    {
        bool ShouldDelayNextFrame { get; }
        void TimingChanged(SystemTimings timings);
        void RenderAudioFrames(Stream data, ulong numFrames);
        void Stop();
    }
}