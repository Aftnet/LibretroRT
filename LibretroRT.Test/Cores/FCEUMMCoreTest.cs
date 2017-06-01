namespace LibretroRT.Test.Cores
{
    public class FCEUMMCoreTest : TestBase
    {
        public FCEUMMCoreTest() : base(() => FCEUMMRT.FCEUMMCore.Instance, "Roms\\SMB3.nes")
        {

        }
    }
}
