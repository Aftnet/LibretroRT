using System;
using System.Runtime.InteropServices;

namespace LibRetriX.RetroBindings
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VFSInterfaceInfo
    {
        private uint requiredInterfaceVersion;
        /// <summary>
        /// Set by core: should this be higher than the version the front end supports,
        /// front end will return false in the RETRO_ENVIRONMENT_GET_VFS_INTERFACE call.
        /// Introduced in VFS API v1.
        /// </summary>
        public uint RequiredInterfaceVersion
        {
            get => requiredInterfaceVersion;
            set { requiredInterfaceVersion = value; }
        }

        private IntPtr vfs_interface;
        /// <summary>
        /// Frontend writes interface function pointers here.
        /// The frontend also sets the actual version, must be at least required_interface_version.
        /// Introduced in VFS API v1.
        /// </summary>
        public IntPtr Interface
        {
            get => vfs_interface;
            set { vfs_interface = value; }
        }
    }
}
