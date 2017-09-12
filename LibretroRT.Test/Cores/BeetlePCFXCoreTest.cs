﻿using System.Threading.Tasks;
using Xunit;

namespace LibretroRT.Test.Cores
{
    [Collection(nameof(TestBase))]
    public class BeetlePCFXCoreTest : TestBase
    {
        protected const string RomName = "PCFXGame.cue";

        public BeetlePCFXCoreTest() : base(() => BeetlePCFXRT.BeetlePCFXCore.Instance)
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
