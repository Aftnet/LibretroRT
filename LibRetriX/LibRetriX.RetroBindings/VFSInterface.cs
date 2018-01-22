using System.Runtime.InteropServices;

namespace LibRetriX.RetroBindings
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VFSInterface
    {
        private VFSGetPathDelegate getPath;
        public VFSGetPathDelegate GetPath
        {
            get => getPath;
            set { getPath = value; }
        }

        private VFSOpenDelegate open;
        public VFSOpenDelegate Open
        {
            get => open;
            set { open = value; }
        }

        private VFSCloseDelegate close;
        public VFSCloseDelegate Close
        {
            get => close;
            set { close = value; }
        }

        private VFSGetSizeDelegate getSize;
        public VFSGetSizeDelegate GetSize
        {
            get => getSize;
            set { getSize = value; }
        }

        private VFSGetPositionDelegate getPosition;
        public VFSGetPositionDelegate GetPosition
        {
            get => getPosition;
            set { getPosition = value; }
        }

        private VFSSetPositionDelegate setPosition;
        public VFSSetPositionDelegate SetPosition
        {
            get => setPosition;
            set { setPosition = value; }
        }

        private VFSReadDelegate read;
        public VFSReadDelegate Read
        {
            get => read;
            set { read = value; }
        }

        private VFSWriteDelegate write;
        public VFSWriteDelegate Write
        {
            get => write;
            set { write = value; }
        }

        private VFSFlushDelegate flush;
        public VFSFlushDelegate Flush
        {
            get => flush;
            set { flush = value; }
        }

        private VFSDeleteDelegate delete;
        public VFSDeleteDelegate Delete
        {
            get => delete;
            set { delete = value; }
        }

        private VFSRenameDelegate rename;
        public VFSRenameDelegate Rename
        {
            get => rename;
            set { rename = value; }
        }
    };
}
