using LibretroRT;
using PCLStorage;
using RetriX.Shared.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RetriX.UWP.ViewModels
{
    public class GameSystemVM : RetriX.Shared.ViewModels.GameSystemVM
    {
        private bool DependenciesOverride { get; set; }

        public ICore Core { get; private set; }
        public IEnumerable<string> MultiFileExtensions { get; private set; }

        public GameSystemVM(ICore core, ILocalizationService localizer, string nameResKey, string manufacturerResKey, string symbol, bool dependenciesOverride = false, IEnumerable<string> supportedExtensionsOverride = null, IEnumerable<string> multiFileExtensions = null) :
            base(localizer, nameResKey, manufacturerResKey, symbol, supportedExtensionsOverride == null ? core.SupportedExtensions : supportedExtensionsOverride)
        {
            DependenciesOverride = dependenciesOverride;

            Core = core;
            MultiFileExtensions = multiFileExtensions == null ? new string[0] : multiFileExtensions;
        }

        public override bool CheckRootFolderRequired(IFile file)
        {
            var extension = Path.GetExtension(file.Name);
            return MultiFileExtensions.Contains(extension);
        }

        public override async Task<bool> CheckDependenciesMetAsync()
        {
            if (DependenciesOverride)
            {
                return true;
            }

            var systemFolder = Core.SystemFolder;
            foreach (var i in Core.FileDependencies)
            {
                try
                {
                    await systemFolder.GetFileAsync(i.Name);
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }
    }
}
