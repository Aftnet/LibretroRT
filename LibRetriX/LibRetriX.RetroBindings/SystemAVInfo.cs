using System.Runtime.InteropServices;

namespace LibRetriX.RetroBindings
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SystemAVInfo
    {
        public GameGeometry Geometry { get; private set; }
        public SystemTimings Timings { get; private set; }
    }
}
