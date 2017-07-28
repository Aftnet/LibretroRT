using System.Threading.Tasks;
using Xunit;

namespace LibretroRT.Test.Cores
{
    public class BeetleNGPCoreTest : TestBase
    {
        protected const string RomName = "Mr Driller.wsc";

        public BeetleNGPCoreTest() : base(() => BeetleNGPRT.BeetleNGPCore.Instance)
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
