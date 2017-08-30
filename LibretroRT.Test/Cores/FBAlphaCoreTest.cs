using System.Threading.Tasks;
using Xunit;

namespace LibretroRT.Test.Cores
{
    public class FBAlphaCoreTest : TestBase
    {
        protected const string RomName = "NeoGeoPocketGame.ngc";

        public FBAlphaCoreTest() : base(() => FBAlphaRT.FBAlphaCore.Instance)
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
