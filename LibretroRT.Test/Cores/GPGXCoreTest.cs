using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LibretroRT.Test.Cores
{
    [Collection(nameof(TestBase))]
    public class GPGXCoreTest : TestBase
    {
        public static IEnumerable<object[]> RomNames => new List<object[]>
        {
            new object[] { "GenesisGame.md" },
            new object[] { "MegaCDGame.chd" },
        };

        public GPGXCoreTest() : base(() => GPGXRT.GPGXCore.Instance)
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
