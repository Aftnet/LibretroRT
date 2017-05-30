namespace LibretroRT.Test
{
    public class NestopiaCoreTest : TestBase
    {
        public NestopiaCoreTest() : base(() => NestopiaRT.NestopiaCore.Instance, "Roms\\SMB3.nes")
        {

        }
    }
}
