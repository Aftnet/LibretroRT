namespace LibretroRT.Test.Cores
{
    public class VBAMCoreTest : TestBase
    {
        public VBAMCoreTest() : base(() => VBAMRT.VBAMCore.Instance, StreamProvider.Scheme + "PokemonFireRed.gba")
        {

        }
    }
}
