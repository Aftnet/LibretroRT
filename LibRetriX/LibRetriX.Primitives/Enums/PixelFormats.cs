namespace LibRetriX
{
    public enum PixelFormats
    {
        /// <summary>
        /// 0RGB1555, native endian, 0 bit must be set to 0.
        /// This pixel format is default for compatibility concerns only.
        /// If a 15/16-bit pixel format is desired, consider using RGB565.
        /// </summary>
        RGB0555 = 0,

        /// <summary>
        /// XRGB8888, native endian. X bits are ignored.
        /// </summary>
        XRGB8888 = 1,

        /// <summary>
        /// RGB565, native endian.
        /// This pixel format is the recommended format to use if a 15/16-bit format is desired
        /// as it is the pixel format that is typically available on a wide range of low-power devices.
        /// It is also natively supported in APIs like OpenGL ES.
        /// </summary>
        RGB565 = 2,

        Unknown = int.MaxValue
    }
}
