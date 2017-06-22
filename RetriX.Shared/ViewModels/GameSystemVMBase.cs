using RetriX.Shared.Services;
using System.Collections.Generic;

namespace RetriX.Shared.ViewModels
{
    public class GameSystemVMBase
    {
        public string Name { get; private set; }
        public string Manufacturer { get; private set; }
        public string Symbol { get; private set; }
        public IReadOnlyList<string> SupportedExtensions { get; private set; }

        public GameSystemVMBase(ILocalizationService localizer, string nameResKey, string manufacturerResKey, string symbol, IReadOnlyList<string> supportedExtensions)
        {
            Name = localizer.GetLocalizedString(nameResKey);
            Manufacturer = localizer.GetLocalizedString(manufacturerResKey);
            Symbol = symbol;
            SupportedExtensions = supportedExtensions;
        }
    }
}
