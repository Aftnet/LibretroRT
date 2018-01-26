using System.Runtime.InteropServices;

namespace LibRetriX
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SystemTimings
    {
        private double fps;
        /// <summary>
        /// FPS of video content
        /// </summary>
        public double FPS => fps;

        private double sampleRate;
        /// <summary>
        /// Sampling rate of audio
        /// </summary>
        public double SampleRate => sampleRate;
    }
}
