namespace LibretroRT.Test.Cores
{
    public class FCEUMMCoreTest : TestBase
    {
        public FCEUMMCoreTest() : base(() => FCEUMMRT.FCEUMMCore.Instance, StreamProvider.Scheme + "SMB3.nes")
        {

        }
    }
}
