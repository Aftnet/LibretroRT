using System.Threading.Tasks;
using Xunit;

namespace LibretroRT.Test.Cores
{
    [Collection(nameof(TestBase))]
    public class BeetlePSXCoreTest : TestBase
    {
        protected const string RomName = "PlayStationGame.cue";

        public BeetlePSXCoreTest() : base(() => BeetlePSXRT.BeetlePSXCore.Instance)
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
