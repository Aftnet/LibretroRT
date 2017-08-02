using System.Threading.Tasks;
using Xunit;

namespace LibretroRT.Test.Cores
{
    public class YabauseCoreTest : TestBase
    {
        protected const string RomName = "SaturnGame.cue";

        public YabauseCoreTest() : base(() => YabauseRT.YabauseCore.Instance)
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
