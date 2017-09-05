using System.Threading.Tasks;
using Xunit;

namespace LibretroRT.Test.Cores
{
    public class FBAlphaCoreTest : TestBase
    {
        protected const string CPS1RomName = "3wondersu.zip";
        protected const string NeoGeoRomName = "mslug.zip";

        public FBAlphaCoreTest() : base(() => FBAlphaRT.FBAlphaCore.Instance)
        {
        }

        [Theory]
        [InlineData(CPS1RomName)]
        [InlineData(NeoGeoRomName)]
        public override Task LoadingRomWorks(string romName)
        {
            return LoadingRomWorksInternal(romName);
        }

        [Theory]
        [InlineData(CPS1RomName)]
        [InlineData(NeoGeoRomName)]
        public override Task ExecutionWorks(string romName)
        {
            return ExecutionWorksInternal(romName);
        }
    }
}
