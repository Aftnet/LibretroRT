namespace LibretroRT.Test.Cores
{
    public class NestopiaCoreTest : TestBase
    {
        public NestopiaCoreTest() : base(() => NestopiaRT.NestopiaCore.Instance, StreamProvider.Scheme + "SMB3.nes")
        {

        }
    }
}
