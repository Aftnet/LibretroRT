using LibRetriX.RetroBindings;
using System;
using System.Threading;

namespace LibRetriX.BeetlePCFX
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
            new FileDependency("pcfx.rom", "PC-FX BIOS", "08e36edbea28a017f79f8d4f7ff9b6d7"),
        };
    }
}
