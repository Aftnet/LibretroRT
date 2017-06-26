using System.Threading.Tasks;
using Xunit;

namespace LibretroRT.Test.Cores
{
    public class NestopiaCoreTest : TestBase
    {
        protected const string RomName = "Super Mario Bros 3.nes";

        public NestopiaCoreTest() : base(() => NestopiaRT.NestopiaCore.Instance)
        {
        }

        [Theory]
        [InlineData(RomName)]
        public override Task LoadingRomWorks(string romName)
        {
            return LoadingRomWorksInternal(romName);
        }

        [Theory]
        [InlineData(RomName)]
        public override Task ExecutionWorks(string romName)
        {
            return ExecutionWorksInternal(romName);
        }
    }
}
