using System;
using System.Runtime.InteropServices;

namespace LibRetriX.RetroBindings
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ControllerInfo
    {
        private IntPtr descriptionsPtr;
        public IntPtr DescriptionsPtr => descriptionsPtr;

        private uint numDescriptions;
        public uint NumDescriptions => numDescriptions;
    };
}
