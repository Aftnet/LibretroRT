namespace LibretroRT.Test.Cores
{
    public class VBAMCoreTest : TestBase
    {
        public VBAMCoreTest() : base(() => VBAMRT.VBAMCore.Instance, "Roms\\PokemonFireRed.gba")
        {

        }
    }
}
