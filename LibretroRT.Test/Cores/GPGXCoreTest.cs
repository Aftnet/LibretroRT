using System.Threading.Tasks;
using Xunit;

namespace LibretroRT.Test.Cores
{
    public class GPGXCoreTest : TestBase
    {
        protected const string RomName = "GenesisGame.md";

        public GPGXCoreTest() : base(() => GPGXRT.GPGXCore.Instance)
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
