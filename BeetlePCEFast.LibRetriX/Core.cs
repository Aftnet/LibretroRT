using LibRetriX.RetroBindings;
using System;
using System.Threading;

namespace LibRetriX.BeetlePCEFast
{
    public static class Core
    {
        private static Lazy<ICore> core = new Lazy<ICore>(InitCore, LazyThreadSafetyMode.ExecutionAndPublication);

        public static ICore Instance => core.Value;

        private static ICore InitCore()
        {
            return new LibretroCore(Dependencies);
        }

        private static readonly FileDependency[] Dependencies =
        {
            new FileDependency("BIOS_CD_E.bin", "Mega-CD (Model 1 1.00 Europe) BIOS", "e66fa1dc5820d254611fdcdba0662372"),
            new FileDependency("BIOS_CD_J.bin", "Mega-CD (Model 1 1.00 Japan) BIOS", "278a9397d192149e84e820ac621a8edd"),
            new FileDependency("BIOS_CD_U.bin", "Mega-CD (Model 1 1.00 USA) BIOS", "2efd74e3232ff260e371b99f84024f7f"),
        };
    }
}
