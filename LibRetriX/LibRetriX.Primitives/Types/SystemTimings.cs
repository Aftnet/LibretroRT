using System.Runtime.InteropServices;

namespace LibRetriX
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SystemTimings
    {
        /// <summary>
        /// FPS of video content
        /// </summary>
        public double FPS { get; private set; }

        /// <summary>
        /// Sampling rate of audio
        /// </summary>
        public double SampleRate { get; private set; }
    }
}
