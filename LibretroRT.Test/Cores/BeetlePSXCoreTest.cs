namespace LibretroRT.Test.Cores
{
    public class BeetlePSXCoreTest : TestBase
    {
        public BeetlePSXCoreTest() : base(() => BeetlePSXRT.BeetlePSXCore.Instance, StreamProvider.Scheme + "Crash Bandicoot - Warped (USA)")
        {
        }
    }
}
