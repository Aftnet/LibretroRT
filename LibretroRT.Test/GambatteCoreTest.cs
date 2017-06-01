namespace LibretroRT.Test
{
    public class GambetteCoreTest : TestBase
    {
        public GambetteCoreTest() : base(() => GambatteRT.GambatteCore.Instance, "Roms\\PokemonSilver.gbc")
        {

        }
    }
}
