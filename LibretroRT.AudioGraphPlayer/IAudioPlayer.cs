using System.Runtime.InteropServices.WindowsRuntime;

namespace LibretroRT.AudioGraphPlayer
{
    public interface IAudioPlayer
    {
        ICore Core { get; set; }

        void ForceDetectSampleRate();
        bool ShouldDelayNextFrame { get; }

        void Stop();
    }
}