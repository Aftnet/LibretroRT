using System.Threading.Tasks;
using Xunit;

namespace LibretroRT.Test.Cores
{
    public class MelonDSCoreTest : TestBase
    {
        protected const string RomName = "Crash Bandicoot 3.cue";

        public MelonDSCoreTest() : base(() => MelonDSRT.MelonDSCore.Instance)
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
