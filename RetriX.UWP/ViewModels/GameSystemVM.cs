using LibretroRT;
using RetriX.Shared.Services;
using RetriX.Shared.ViewModels;
using System.Collections.Generic;

namespace RetriX.UWP.ViewModels
{
    public class GameSystemVM : GameSystemVMBase
    {
        public ICore Core { get; private set; }
        public IEnumerable<string> MultiFileExtensions { get; private set; }

        public GameSystemVM(ICore core, ILocalizationService localizer, string nameResKey, string manufacturerResKey, string symbol, IEnumerable<string> supportedExtensionsOverride = null, IEnumerable<string> multiFileExtensions = null) :
            base(localizer, nameResKey, manufacturerResKey, symbol, supportedExtensionsOverride == null ? core.SupportedExtensions : supportedExtensionsOverride)
        {
            Core = core;
            MultiFileExtensions = multiFileExtensions == null ? new string[0] : multiFileExtensions;
        }
    }
}
