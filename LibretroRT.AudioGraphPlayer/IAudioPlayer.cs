using System.Runtime.InteropServices.WindowsRuntime;

namespace LibretroRT.AudioGraphPlayer
{
    public interface IAudioPlayer
    {
        ICore Core { get; set; }

        bool ShouldDelayNextFrame { get; }

        void Stop();
    }
}