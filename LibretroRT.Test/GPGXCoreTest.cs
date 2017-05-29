namespace LibretroRT.Test
{
    public class GPGXCoreTest : TestBase
    {
        public GPGXCoreTest() : base(() => GPGXRT.GPGXCore.Instance, "Roms\\Sonic2.md")
        {

        }
    }
}
