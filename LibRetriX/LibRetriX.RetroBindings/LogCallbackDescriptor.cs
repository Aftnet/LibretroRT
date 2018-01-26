using System.Runtime.InteropServices;

namespace LibRetriX.RetroBindings
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LogCallbackDescriptor
    {
        private LibretroLogDelegate logCallback;
        public LibretroLogDelegate LogCallback
        {
            get => logCallback;
            set { logCallback = value; }
        }
    }
}
