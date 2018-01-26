namespace LibRetriX.RetroBindings
{
    public enum Modifiers
    {
        RETROKMOD_NONE = 0x0000,

        RETROKMOD_SHIFT = 0x01,
        RETROKMOD_CTRL = 0x02,
        RETROKMOD_ALT = 0x04,
        RETROKMOD_META = 0x08,

        RETROKMOD_NUMLOCK = 0x10,
        RETROKMOD_CAPSLOCK = 0x20,
        RETROKMOD_SCROLLOCK = 0x40,

        RETROKMOD_DUMMY = int.MaxValue /* Ensure sizeof(enum) == sizeof(int) */
    };
}
