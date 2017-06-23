namespace LibretroRT.Test.Cores
{
    public class GPGXCoreTest : TestBase
    {
        public GPGXCoreTest() : base(() => GPGXRT.GPGXCore.Instance, StreamProvider.Scheme + "Sonic2.md")
        {

        }
    }
}
