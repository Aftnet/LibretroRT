using System.Runtime.InteropServices;

namespace LibRetriX
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GameGeometry
    {
        private uint baseWidth;
        /// <summary>
        /// Nominal video width of game
        /// </summary>
        public uint BaseWidth => baseWidth;

        private uint baseHeight;
        /// <summary>
        /// Nominal video height of game
        /// </summary>
        public uint BaseHeight => baseHeight;

        private uint maxWidth;
        /// <summary>
        /// Maximum possible width of game
        /// </summary>
        public uint MaxWidth => maxWidth;

        private uint maxHeight;
        /// <summary>
        /// Maximum possible height of game
        /// </summary>
        public uint MaxHeight => maxHeight;

        private float aspectRatio;
        /// <summary>
        /// Nominal aspect ratio of game.
        /// If aspect_ratio is <= 0.0, an aspect ratio
        /// of base_width / base_height is assumed.
        /// A frontend could override this setting
        /// if desired
        /// </summary>
        public float AspectRatio => aspectRatio;
    }
}
