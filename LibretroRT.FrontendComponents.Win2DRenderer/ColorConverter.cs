using System;

namespace LibretroRT.FrontendComponents.Win2DRenderer
{
    internal static class ColorConverter
    {
        private static readonly int[] RGB565LUT = new int[ushort.MaxValue];

        static ColorConverter()
        {
            int r, g, b;
            unchecked
            {
                for (ushort i = 0; i < ushort.MaxValue; i++)
                {
                    r = ((i >> 11) & 0x1F);
                    g = ((i >> 5) & 0x3F);
                    b = (i & 0x1F);

                    r = ((((i >> 11) & 0x1F) * 527) + 23) >> 6;
                    g = ((((i >> 5) & 0x3F) * 259) + 33) >> 6;
                    b = (((i & 0x1F) * 527) + 23) >> 6;

                    RGB565LUT[i] = b << 24 | g << 16 | r << 8 | 0xff;
                }
            }
        }

        unsafe public static void ConvertFrameBufferRGB565ToXRGB8888(byte[] input, uint width, uint height, uint pitch, byte[] output)
        {
            if (output.Length < input.Length * 2)
            {
                throw new ArgumentException();
            }

            fixed (byte* inPtr = input)
            fixed (byte* outPtr = output)
            fixed (int* lutPtr = RGB565LUT)
            {
                var outIntPtr = (int*)outPtr;
                var inLineStart = inPtr;

                for (var i = 0; i < height; i++)
                {
                    var inShortPtr = (ushort*)inLineStart;

                    unchecked
                    {
                        for (var j = 0; j < width; j++)
                        {
                            *outIntPtr = lutPtr[inShortPtr[j]];
                            outIntPtr++;
                        }
                    }

                    inLineStart += pitch;
                }
            }
        }
    }
}
