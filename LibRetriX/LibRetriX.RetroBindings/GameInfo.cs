using System;
using System.Runtime.InteropServices;

namespace LibRetriX.RetroBindings
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct GameInfo
    {
        [MarshalAs(UnmanagedType.LPStr)]
        private string path;
        /// <summary>
        /// Path to game. Sometimes used as a reference for building other paths.
        /// May be NULL if game was loaded from stdin or similar, but in this case some cores will be unable to load `data`.
        /// So, it is preferable to fabricate something here instead of passing NULL, which will help more cores to succeed.
        /// retro_system_info::need_fullpath requires that this path is valid.
        /// </summary>
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        private IntPtr dataPtr;
        private IntPtr dataSize;
        /// <summary>
        /// Set game data to the specified array
        /// </summary>
        /// <param name="data">Byte array holding game data</param>
        /// <returns>Handle to pinned data. Release when unloading game.</returns>
        public GCHandle SetGameData(byte[] data)
        {
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            dataPtr = handle.AddrOfPinnedObject();
            dataSize = (IntPtr)data.Length;
            return handle;
        }

        private IntPtr meta;
    }
}
