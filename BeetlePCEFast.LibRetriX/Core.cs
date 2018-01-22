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
            var core = new LibretroCore(Dependencies);
            core.Initialize();
            return core;
        }

        private static readonly FileDependency[] Dependencies =
        {
            new FileDependency("syscard3.pce", "PC Engine CD BIOS", "ff1a674273fe3540ccef576376407d1d"),
        };
    }
}
