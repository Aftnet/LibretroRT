namespace LibRetroRT.AV
{
    public struct GameGeometry
    {
        public uint BaseWidth;    /* Nominal video width of game. */
        public uint BaseHeight;   /* Nominal video height of game. */
        public uint MaxWidth;     /* Maximum possible width of game. */
        public uint MaxHeight;    /* Maximum possible height of game. */

        public float AspectRatio;  /* Nominal aspect ratio of game. If
                            * aspect_ratio is <= 0.0, an aspect ratio
                            * of base_width / base_height is assumed.
                            * A frontend could override this setting,
                            * if desired. */
    }
}
