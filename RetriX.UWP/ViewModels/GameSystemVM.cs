using LibretroRT;
using RetriX.Shared.Services;
using RetriX.Shared.ViewModels;
using System.Collections.Generic;

namespace RetriX.UWP.ViewModels
{
    public class GameSystemVM : GameSystemVMBase
    {
        public ICore Core { get; private set; }

        public GameSystemVM(ICore core, ILocalizationService localizer, string nameResKey, string manufacturerResKey, string symbol, IReadOnlyList<string> supportedExtensions) : base(localizer, nameResKey, manufacturerResKey, symbol, supportedExtensions)
        {
            Core = core;
        }
    }
}
