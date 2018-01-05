﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LibretroRT.Test.Cores
{
    public class GambetteCoreTest : TestBase
    {
        public static IEnumerable<object[]> RomNames => new List<object[]>
        {
            new object[] { "GBCGame.gbc" },
        };

        public GambetteCoreTest() : base(() => GambatteRT.GambatteCore.Instance)
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
