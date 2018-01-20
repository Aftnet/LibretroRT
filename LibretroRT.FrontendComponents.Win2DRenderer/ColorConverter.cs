using LibretroRT.FrontendComponents.Common;
using System;
using System.IO;

namespace LibretroRT.FrontendComponents.Win2DRenderer
{
    internal static class ColorConverter
    {
        private const uint LookupTableSize = ushort.MaxValue + 1;

        private static readonly uint[] RGB565LookupTable = new uint[LookupTableSize];

        static ColorConverter()
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
        }

        unsafe public static void ConvertFrameBufferRGB565ToXRGB8888(Stream input, uint width, uint height, ulong pitch, byte[] output)
        {
            if (output.Length < input.Length * 2)
            {
                throw new ArgumentException();
            }

            input.TryGetPointer(out var inputPtr, out var handle);
            fixed (byte* outPtr = output)
            fixed (uint* lutPtr = RGB565LookupTable)
            {
                var outIntPtr = (uint*)outPtr;
                var inLineStart = (byte*)inputPtr;

                for (var i = 0; i < height; i++)
                {
                    var inShortPtr = (ushort*)inLineStart;

                    for (var j = 0; j < width; j++)
                    {
                        *outIntPtr = lutPtr[inShortPtr[j]];
                        outIntPtr++;
                    }

                    inLineStart += pitch;
                }
            }

            if (handle.IsAllocated)
            {
                handle.Free();
            }
        }
    }
}
