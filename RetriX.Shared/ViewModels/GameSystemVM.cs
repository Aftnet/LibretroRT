using PCLStorage;
using RetriX.Shared.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetriX.Shared.ViewModels
{
    public class GameSystemVM
    {
        public string Name { get; private set; }
        public string Manufacturer { get; private set; }
        public string Symbol { get; private set; }
        public IEnumerable<string> SupportedExtensions { get; private set; }

        public GameSystemVM(ILocalizationService localizer, string nameResKey, string manufacturerResKey, string symbol, IEnumerable<string> supportedExtensions)
        {
            Name = localizer.GetLocalizedString(nameResKey);
            Manufacturer = localizer.GetLocalizedString(manufacturerResKey);
            Symbol = symbol;
            SupportedExtensions = supportedExtensions;
        }

        public virtual Task<bool> CheckDependenciesMetAsync()
        {
            return Task.FromResult(true);
        }

        public virtual bool CheckRootFolderRequired(IFile file)
        {
            return false;
        }
    }
}
