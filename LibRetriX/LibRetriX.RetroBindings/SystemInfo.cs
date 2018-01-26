using System;
using System.Runtime.InteropServices;

namespace LibRetriX.RetroBindings
{
    /// <summary>
    /// All pointers are owned by libretro implementation, and pointers must
    /// remain valid until retro_deinit() is called.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SystemInfo
    {
        private IntPtr libraryName;
        /// <summary>
        /// Descriptive name of library. Should not contain any version numbers, etc.
        /// </summary>
        public string LibraryName => Marshal.PtrToStringAnsi(libraryName);

        private IntPtr libraryVersion;
        /// <summary>
        /// Descriptive version of core.
        /// </summary>
        public string LibraryVersion => Marshal.PtrToStringAnsi(libraryVersion);

        private IntPtr validExtensions;
        /// <summary>
        /// A string listing probably content extensions the core will be able to load, separated with pipe.
        /// I.e. "bin|rom|iso".
        /// Typically used for a GUI to filter out extensions.
        /// </summary>
        public string ValidExtensions => Marshal.PtrToStringAnsi(validExtensions);

        [MarshalAs(UnmanagedType.I1)]
        private bool needFullpath;
        /// <summary>
        /// If true, retro_load_game() is guaranteed to provide a valid pathname in path. data and size are both invalid.
        /// If false, data and size are guaranteed to be valid, path might not be valid.
        /// This is typically set to true for libretro implementations that must load from file.
        /// Implementations should strive for setting this to false, as it allows the frontend to perform patching, etc.
        /// </summary>
        public bool NeedFullpath => needFullpath;

        [MarshalAs(UnmanagedType.I1)]
        private bool blockExtract;
        /// <summary>
        /// If true, the frontend is not allowed to extract any archives before loading the real content.
        /// Necessary for certain libretro implementations that load games from zipped archives.
        /// </summary>
        public bool BlockExtract => blockExtract;
    };
}
