using System.Runtime.InteropServices;

namespace LibRetriX.RetroBindings
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SystemAVInfo
    {
        private GameGeometry geometry;
        public GameGeometry Geometry => geometry;

        private SystemTimings timings;
        public SystemTimings Timings => timings;
    }
}
