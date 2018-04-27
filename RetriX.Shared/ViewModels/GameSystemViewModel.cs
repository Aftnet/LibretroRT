using LibRetriX;
using Plugin.FileSystem.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RetriX.Shared.ViewModels
{
    public class GameSystemViewModel
    {
        private IFileSystem FileSystem { get; }

        public ICore Core { get; }

        public string Name { get; }
        public string Manufacturer { get; }
        public string Symbol { get; }
        public IEnumerable<string> SupportedExtensions { get; }
        public IEnumerable<string> MultiFileExtensions { get; }

        private readonly bool DependenciesOverride;

        public GameSystemViewModel(ICore core, IFileSystem fileSystem, string name, string manufacturer, string symbol, bool dependenciesOverride = false, IEnumerable<string> supportedExtensionsOverride = null, IEnumerable<string> multiFileExtensions = null)
        {
            FileSystem = fileSystem;

            Core = core;
            Name = name;
            Manufacturer = manufacturer;
            Symbol = symbol;
            SupportedExtensions = supportedExtensionsOverride != null ? supportedExtensionsOverride : Core.SupportedExtensions;
            MultiFileExtensions = multiFileExtensions == null ? new string[0] : multiFileExtensions;
            DependenciesOverride = dependenciesOverride;
        }

        public bool CheckRootFolderRequired(IFileInfo file)
        {
            var extension = Path.GetExtension(file.Name);
            return MultiFileExtensions.Contains(extension);
        }

        public async Task<bool> CheckDependenciesMetAsync()
        {
            if (DependenciesOverride)
            {
                return true;
            }

            var systemFolder = await GetSystemDirectoryAsync();
            foreach (var i in Core.FileDependencies)
            {
                var file = await systemFolder.GetFileAsync(i.Name);
                if (file == null)
                {
                    return false;
                }
            }

            return true;
        }

        public Task<IDirectoryInfo> GetSystemDirectoryAsync()
        {
            return GetCoreStorageDirectoryAsync($"{Core.Name} - System");
        }

        public Task<IDirectoryInfo> GetSaveDirectoryAsync()
        {
            return GetCoreStorageDirectoryAsync($"{Core.Name} - Saves");
        }

        private async Task<IDirectoryInfo> GetCoreStorageDirectoryAsync(string directoryName)
        {
            var output = await FileSystem.LocalStorage.GetDirectoryAsync(directoryName);
            if (output == null)
            {
                output = await FileSystem.LocalStorage.CreateDirectoryAsync(directoryName);
            }

            return output;
        }
    }
}
