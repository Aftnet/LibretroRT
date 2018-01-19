using System.Runtime.InteropServices;

namespace LibRetriX
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GameGeometry
    {
        /// <summary>
        /// Nominal video width of game
        /// </summary>
        public uint BaseWidth { get; private set; }

        /// <summary>
        /// Nominal video height of game
        /// </summary>
        public uint BaseHeight { get; private set; }

        /// <summary>
        /// Maximum possible width of game
        /// </summary>
        public uint MaxWidth { get; private set; }

        /// <summary>
        /// Maximum possible height of game
        /// </summary>
        public uint MaxHeight { get; private set; }

        /// <summary>
        /// Nominal aspect ratio of game.
        /// If aspect_ratio is <= 0.0, an aspect ratio
        /// of base_width / base_height is assumed.
        /// A frontend could override this setting
        /// if desired
        /// </summary>
        public float AspectRatio { get; private set; }
    }
}
