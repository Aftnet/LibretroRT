using System.Runtime.InteropServices;

namespace LibRetriX.RetroBindings
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LogCallbackDescriptor
    {
        public LibretroLogDelegate LogCallback { get; set; }
    }
}
