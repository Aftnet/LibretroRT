using System;
using System.Runtime.InteropServices;

namespace RetriX.UWP.Components
{
    internal unsafe static class FramebufferConverter
    {
        private const uint LookupTableSize = ushort.MaxValue + 1;

        private static readonly uint[] RGB565LookupTable = new uint[LookupTableSize];
        private static readonly GCHandle RGB565LookupTableHandle;
        private static readonly uint* RGB565LookupTablePtr;

        static FramebufferConverter()
        {
            uint r, g, b;
            for (uint i = 0; i < LookupTableSize; i++)
            {
                r = (i >> 11) & 0x1F;
                g = (i >> 5) & 0x3F;
                b = (i & 0x1F);

                r = (uint)Math.Round(r * 255.0 / 31.0);
                g = (uint)Math.Round(g * 255.0 / 63.0);
                b = (uint)Math.Round(b * 255.0 / 31.0);

                RGB565LookupTable[i] = 0xFF000000 | r << 16 | g << 8 | b;
            }

            RGB565LookupTableHandle = GCHandle.Alloc(RGB565LookupTable, GCHandleType.Pinned);
            RGB565LookupTablePtr = (uint*)RGB565LookupTableHandle.AddrOfPinnedObject();
        }

        public static void ConvertFrameBufferXRGB8888(uint width, uint height, byte* input, int inputPitch, byte* output, int outputPitch)
        {
            for (var i = 0; i < height; i++)
            {
                Buffer.MemoryCopy(input, output, outputPitch, width * sizeof(uint));
                input += inputPitch;
                output += outputPitch;
            }
        }

        public static void ConvertFrameBufferRGB565ToXRGB8888(uint width, uint height, byte* input, int inputPitch, byte* output, int outputPitch)
        {
            for (var i = 0; i < height; i++)
            {
                var inLineStart = (ushort*)input;
                var outLineStart = (uint*)output;

                for (var j = 0; j < width; j++)
                {
                    outLineStart[j] = RGB565LookupTablePtr[inLineStart[j]];
                }

                input += inputPitch;
                output += outputPitch;
            }
        }
    }
}
