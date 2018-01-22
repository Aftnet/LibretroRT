using LibRetriX.RetroBindings;
using System;
using System.Threading;

namespace LibRetriX.MelonDS
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
            new FileDependency("bios7.bin", "Nintendo DS ARM7 BIOS", "df692a80a5b1bc90728bc3dfc76cd948"),
            new FileDependency("bios9.bin", "Nintendo DS ARM9 BIOS", "a392174eb3e572fed6447e956bde4b25"),
	        new FileDependency("firmware.bin", "Nintendo DS Firmware", "b10f39a8a5a573753406f9da2e7232c8"),
        };
    }
}
