using System;
using System.Runtime.InteropServices;

namespace LibRetriX.RetroBindings
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Framebuffer
    {
        /// <summary>
        /// The framebuffer which the core can render into.
        /// Set by frontend in GET_CURRENT_SOFTWARE_FRAMEBUFFER.
        /// The initial contents of data are unspecified.
        /// </summary>
        public IntPtr Data { get; set; }

        /// <summary>
        /// The framebuffer width used by the core. Set by core.
        /// </summary>
        public uint Width { get; set; }

        /// <summary>
        /// The framebuffer height used by the core. Set by core.
        /// </summary>
        public uint Height { get; set; }

        /// <summary>
        /// The number of bytes between the beginning of a scanline,
        /// and beginning of the next scanline.
        /// Set by frontend in GET_CURRENT_SOFTWARE_FRAMEBUFFER.
        /// </summary>
        public IntPtr Pitch { get; set; }

        /// <summary>
        /// The pixel format the core must use to render into data.
        /// This format could differ from the format used in SET_PIXEL_FORMAT.
        /// Set by frontend in GET_CURRENT_SOFTWARE_FRAMEBUFFER.
        /// </summary>
        public PixelFormats Format { get; set; }

        /// <summary>
        /// How the core will access the memory in the framebuffer.
        /// RETRO_MEMORY_ACCESS_* flags.
        /// Set by core. */
        /// </summary>
        public uint AccessFlags { get; set; }

        /// <summary>
        /// Flags telling core how the memory has been mapped.
        /// RETRO_MEMORY_TYPE_* flags.
        /// Set by frontend in GET_CURRENT_SOFTWARE_FRAMEBUFFER
        /// </summary>
        public uint MemoryFlags { get; set; }
    }
}
