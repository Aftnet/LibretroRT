using System.Threading.Tasks;
using Xunit;

namespace LibretroRT.Test.Cores
{
    public class Snes9XCoreTest : TestBase
    {
        protected const string RomName = "SNESGame.sfc";

        public Snes9XCoreTest() : base(() => Snes9XRT.Snes9XCore.Instance)
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
