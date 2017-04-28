using System.Collections.Generic;

namespace LibRetroRT.AV
{
    public sealed class Framebuffer
    {
        private IReadOnlyList<byte> data;
        public IReadOnlyList<byte> Data
        {
            get { return data; }
            set { data = value; }
        }

        private uint width;                  /* The framebuffer width used by the core. Set by core. */
        private uint Width
        {
            get { return width; }
            set { width = value; }
        }

        private uint height;                 /* The framebuffer height used by the core. Set by core. */
        private uint Height
        {
            get { return height; }
            set { height = value; }
        }

        private ulong pitch;                   /* The number of bytes between the beginning of a scanline,
                                               and beginning of the next scanline.
                                               Set by frontend in GET_CURRENT_SOFTWARE_FRAMEBUFFER. */
        private ulong Pitch
        {
            get { return pitch; }
            set { pitch = value; }
        }

        private PixelFormats pixelFormat;
        private PixelFormats PixelFormat
        {
            get { return pixelFormat; }
            set { pixelFormat = value; }
        }
    }
}
