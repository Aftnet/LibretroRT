using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LibretroRT.Test.Cores
{
    public class FBAlphaCoreTest : TestBase
    {
        public static IEnumerable<object[]> RomNames => new List<object[]>
        {
            new object[] { "mslug.zip" },
            new object[] { "dkong.zip" },
            new object[] { "3wondersu.zip" },
            new object[] { "xmcota.zip" },
            new object[] { "sfiii.zip" }
        };

        public FBAlphaCoreTest() : base(() => FBAlphaRT.FBAlphaCore.Instance)
        {
        }

        [Theory]
        [MemberData(nameof(RomNames))]
        public override Task LoadingRomWorks(string romName)
        {
            return LoadingRomWorksInternal(romName);
        }

        [Theory]
        [MemberData(nameof(RomNames))]
        public override Task ExecutionWorks(string romName)
        {
            return ExecutionWorksInternal(romName);
        }
    }
}
