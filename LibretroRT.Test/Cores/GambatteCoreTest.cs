using System.Threading.Tasks;
using Xunit;

namespace LibretroRT.Test.Cores
{
    public class GambetteCoreTest : TestBase
    {
        protected const string RomName = "Pokemon Silver.gbc";

        public GambetteCoreTest() : base(() => GambatteRT.GambatteCore.Instance)
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
