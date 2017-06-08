using System;

namespace LibretroRT.FrontendComponents.Win2DRenderer
{
    internal static class ColorConverter
    {
        private static readonly int[] RGB565LUT = new int[ushort.MaxValue];

        static ColorConverter()
        {
            int r, g, b;
            for (ushort i = 0; i < ushort.MaxValue; i++)
            {
                r = ((i >> 11) & 0x1F);
                g = ((i >> 5) & 0x3F);
                b = (i & 0x1F);

                r = ((((i >> 11) & 0x1F) * 527) + 23) >> 6;
                g = ((((i >> 5) & 0x3F) * 259) + 33) >> 6;
                b = (((i & 0x1F) * 527) + 23) >> 6;

                RGB565LUT[i] = 0xff << 24 | r << 16 | g << 8 | b;
            }
        }

        unsafe public static void ConvertFrameBufferRGB565ToXRGB8888(byte[] input, uint width, uint height, uint pitch, byte[] output, uint outputPitch)
        {
            if (output.Length < input.Length * 2)
            {
                throw new ArgumentException();
            }

            fixed (byte* inPtr = input)
            fixed (byte* outPtr = output)
            {
                var inLineStart = inPtr;
                var outLineStart = outPtr;

                for (var i = 0; i < height; i++)
                {
                    ushort* inShortPtr = (ushort*)inPtr;
                    int* outIntPtr = (int*)outPtr;

                    for (var j = 0; j < width; j++)
                    {
                        outIntPtr[j] = RGB565LUT[inShortPtr[j]];
                    }

                    inLineStart += pitch;
                    outLineStart += outputPitch;
                }
            }
        }
    }
}
