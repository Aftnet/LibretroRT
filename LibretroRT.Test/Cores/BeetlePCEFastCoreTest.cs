using System.Threading.Tasks;
using Xunit;

namespace LibretroRT.Test.Cores
{
    public class BeetlePCEFastCoreTest : TestBase
    {
        protected const string PCERomName = "PCEngineGame.pce";
        protected const string PCECDRomName = "PCEngineCDGame.chd";

        public BeetlePCEFastCoreTest() : base(() => BeetleNGPRT.BeetleNGPCore.Instance)
        {
        }

        [Theory]
        [InlineData(PCERomName)]
        [InlineData(PCECDRomName)]
        public override Task LoadingRomWorks(string romName)
        {
            return LoadingRomWorksInternal(romName);
        }

        [Theory]
        [InlineData(PCERomName)]
        [InlineData(PCECDRomName)]
        public override Task ExecutionWorks(string romName)
        {
            return ExecutionWorksInternal(romName);
        }
    }
}
