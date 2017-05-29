namespace LibretroRT.Test
{
    public class VBAMCoreTest : TestBase
    {
        public VBAMCoreTest() : base(() => VBAMRT.VBAMCore.Instance, "Roms\\PokemonFireRed.gba")
        {

        }
    }
}
