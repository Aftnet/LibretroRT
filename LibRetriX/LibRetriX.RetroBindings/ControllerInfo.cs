using System;
using System.Runtime.InteropServices;

namespace LibRetriX.RetroBindings
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ControllerInfo
    {
        public IntPtr DescriptionsPtr { get; private set; }
        public uint NumDescriptions { get; private set; }
    };
}
