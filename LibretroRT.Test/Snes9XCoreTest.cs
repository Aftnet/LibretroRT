namespace LibretroRT.Test
{
    public class Snes9XCoreTest : TestBase
    {
        public Snes9XCoreTest() : base(() => Snes9XRT.Snes9XCore.Instance, "Roms\\SMW.sfc")
        {

        }
    }
}
