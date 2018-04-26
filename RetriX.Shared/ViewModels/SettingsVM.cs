using Acr.UserDialogs;
using LibRetriX;
using MvvmCross.Core.ViewModels;
using Plugin.FileSystem.Abstractions;
using RetriX.Shared.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetriX.Shared.ViewModels
{
    public class SettingsVM : MvxViewModel
    {
        private IEmulationService EmulationService { get; }
        private IFileSystem FileSystem { get; }
        private IUserDialogs DialogsService { get; }
        private IPlatformService PlatformService { get; }
        private ICryptographyService CryptographyService { get; }

        private IReadOnlyList<FileImporterVM> fileDependencyImporters;
        public IReadOnlyList<FileImporterVM> FileDependencyImporters
        {
            get => fileDependencyImporters;
            private set => SetProperty(ref fileDependencyImporters, value);
        }

        public SettingsVM(IEmulationService emulationService, IFileSystem fileSystem, IUserDialogs dialogsService, IPlatformService platformService, ICryptographyService cryptographyService)
        {
            EmulationService = emulationService;
            FileSystem = fileSystem;
            DialogsService = dialogsService;
            PlatformService = platformService;
            CryptographyService = cryptographyService;

            Task.Run(GetFileDependencyImportersAsync).ContinueWith(d =>
            {
                PlatformService.RunOnUIThreadAsync(() => FileDependencyImporters = d.Result);
            });
        }

        private async Task<List<FileImporterVM>> GetFileDependencyImportersAsync()
        {
            var importers = new List<FileImporterVM>();
            var distinctCores = new HashSet<ICore>();
            foreach (var i in EmulationService.Systems)
            {
                var core = i.Core;
                if (distinctCores.Contains(core))
                {
                    continue;
                }

                distinctCores.Add(core);
                var systemFolder = await i.GetSystemDirectoryAsync();
                var tasks = core.FileDependencies.Select(d => FileImporterVM.CreateFileImporterAsync(FileSystem, DialogsService, PlatformService, CryptographyService, systemFolder, d.Name, d.Description, d.MD5)).ToArray();
                var newImporters = await Task.WhenAll(tasks);
                importers.AddRange(newImporters);
            }

            return importers;
        }
    }
}
