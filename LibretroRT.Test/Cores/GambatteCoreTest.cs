﻿namespace LibretroRT.Test.Cores
{
    public class GambetteCoreTest : TestBase
    {
        public GambetteCoreTest() : base(() => GambatteRT.GambatteCore.Instance, StreamProvider.Scheme + "PokemonSilver.gbc")
        {

        }
    }
}
