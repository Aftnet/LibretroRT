using LibRetriX;
using System.IO;

namespace RetriX.Shared.Services
{
    public interface IAudioService : IInitializable
    {
        bool ShouldDelayNextFrame { get; }
        void TimingChanged(SystemTimings timings);
        void RenderAudioFrames(Stream data, ulong numFrames);
        void Stop();
    }
}