namespace LibretroRT.Test.Cores
{
    public class Snes9XCoreTest : TestBase
    {
        public Snes9XCoreTest() : base(() => Snes9XRT.Snes9XCore.Instance, StreamProvider.Scheme + "SMW.sfc")
        {

        }
    }
}
