using System.Threading.Tasks;
using Xunit;

namespace LibretroRT.Test.Cores
{
    public class BeetlePCEFastCoreTest : TestBase
    {
        protected const string RomName = "PCEngineGame.pce";

        public BeetlePCEFastCoreTest() : base(() => BeetleNGPRT.BeetleNGPCore.Instance)
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
