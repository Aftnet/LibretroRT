using LibRetriX.RetroBindings;
using System;
using System.Threading;

namespace LibRetriX.BeetleSaturn
{
    public static class Core
    {
        private static Lazy<ICore> core = new Lazy<ICore>(InitCore, LazyThreadSafetyMode.ExecutionAndPublication);

        public static ICore Instance => core.Value;

        private static ICore InitCore()
        {
            return new LibretroCore();
        }
    }
}
