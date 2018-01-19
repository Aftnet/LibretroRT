using System.Runtime.InteropServices;

namespace LibRetriX.RetroBindings
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VFSInterface
    {
        public VFSGetPathDelegate GetPath { get; set; }
        public VFSOpenDelegate Open { get; set; }
        public VFSCloseDelegate Close { get; set; }
        public VFSGetSizeDelegate GetSize { get; set; }
        public VFSGetPositionDelegate GetPosition { get; set; }
        public VFSSetPositionDelegate SetPosition { get; set; }
        public VFSReadDelegate Read { get; set; }
        public VFSWriteDelegate Write { get; set; }
        public VFSFlushDelegate Flush { get; set; }
        public VFSDeleteDelegate Delete { get; set; }
        public VFSRenameDelegate Rename { get; set; }
    };
}
