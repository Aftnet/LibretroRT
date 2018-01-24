using LibRetriX;
using System;

namespace RetriX.UWP
{
    public interface IAudioPlayer
    {
        bool ShouldDelayNextFrame { get; }
        void TimingChanged(SystemTimings timings);
        void RenderAudioFrames(IntPtr data, ulong numFrames);
        void Stop();
    }
}