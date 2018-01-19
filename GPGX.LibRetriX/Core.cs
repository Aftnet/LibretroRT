using LibRetriX.RetroBindings;
using System;
using System.Threading;

namespace LibRetriX.BeetlePSX
{
    public static class Core
    {
        private static Lazy<ICore> core = new Lazy<ICore>(InitCore, LazyThreadSafetyMode.ExecutionAndPublication);

        public static ICore Instance => core.Value;

        private static ICore InitCore()
        {
            return new LibretroCore(Dependencies, OptionSetters, 1);
        }

        private static readonly FileDependency[] Dependencies =
        {
            new FileDependency("scph5500.bin", "PlayStation (v3.0 09/09/96 J) BIOS", "8dd7d5296a650fac7319bce665a6a53c"),
            new FileDependency("scph5501.bin", "PlayStation (v3.0 11/18/96 A) BIOS", "490f666e1afb15b7362b406ed1cea246"),
            new FileDependency("scph5502.bin", "PlayStation (v3.0 01/06/97 E) BIOS", "32736f17079d0b2b7024407c39bd3050"),
        };

        private static readonly Tuple<string, uint>[] OptionSetters =
        {
            Tuple.Create("beetle_psx_frame_duping_enable", 1U),
            Tuple.Create("beetle_psx_analog_calibration", 1U),
            Tuple.Create("beetle_psx_skipbios", 1U)
        };
    }
}
