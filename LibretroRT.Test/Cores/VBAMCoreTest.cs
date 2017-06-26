using System.Threading.Tasks;
using Xunit;

namespace LibretroRT.Test.Cores
{
    public class VBAMCoreTest : TestBase
    {
        protected const string RomName = "Pokemon Fire Red.gba";

        public VBAMCoreTest() : base(() => VBAMRT.VBAMCore.Instance)
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
