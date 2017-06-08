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

                RGB565LUT[i] = r << 16 | g << 8 | b;
            }
        }

        unsafe public static void ConvertRGB565ToXRGB8888(byte[] input, byte[] output)
        {
            if (output.Length < input.Length * 2)
            {
                throw new ArgumentException();
            }

            fixed (byte* inPtr = input)
            fixed (byte* outPtr = output)
            {
                ushort* inShortPtr = (ushort*)inPtr;
                int* outIntPtr = (int*)outPtr;

                for (var i = 0; i < input.Length / 2; i++)
                {
                    outIntPtr[i] = RGB565LUT[inShortPtr[i]];
                }
            }
        }
    }
}
