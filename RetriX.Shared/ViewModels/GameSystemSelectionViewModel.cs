using Acr.UserDialogs;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using Plugin.FileSystem.Abstractions;
using RetriX.Shared.Services;
using RetriX.Shared.StreamProviders;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RetriX.Shared.ViewModels
{
    public class GameSystemSelectionViewModel : MvxViewModel<IFileInfo>
    {
        private IMvxNavigationService NavigationService { get; }
        private IFileSystem FileSystem { get; }
        private IUserDialogs DialogsService { get; }
        private IPlatformService PlatformService { get; }
        private IEmulationService EmulationService { get; }

        private IFileInfo SelectedGameFile { get; set; }

        private IReadOnlyList<GameSystemViewModel> gameSystems;
        public IReadOnlyList<GameSystemViewModel> GameSystems
        {
            get => gameSystems;
            private set => SetProperty(ref gameSystems, value);
        }

        public IMvxCommand<GameSystemViewModel> GameSystemSelected { get; }

        public GameSystemSelectionViewModel(IMvxNavigationService navigationService, IFileSystem fileSystem, IUserDialogs dialogsService, IPlatformService platformService, IEmulationService emulationService)
        {
            NavigationService = navigationService;
            FileSystem = fileSystem;
            DialogsService = dialogsService;
            PlatformService = platformService;
            EmulationService = emulationService;

            GameSystems = EmulationService.Systems;
            GameSystemSelected = new MvxCommand<GameSystemViewModel>(GameSystemSelectedHandler);
        }

        public override void Prepare(IFileInfo parameter)
        {
            SelectedGameFile = parameter;
        }

        public override async Task Initialize()
        {
            //Find compatible systems for file extension
            var extension = SelectedGameFile != null ? Path.GetExtension(SelectedGameFile.Name) : string.Empty;
            var compatibleSystems = EmulationService.Systems.Where(d => d.SupportedExtensions.Contains(extension));

            //If none, do nothing
            if (!compatibleSystems.Any())
            {
                return;
            }

            //If just one, start game with it
            if (compatibleSystems.Count() == 1)
            {
                await StartGameAsync(compatibleSystems.Single(), SelectedGameFile);
                return;
            }

            GameSystems = compatibleSystems.ToArray();
        }

        private async void GameSystemSelectedHandler(GameSystemViewModel system)
        {
            if (SelectedGameFile == null)
            {
                var extensions = system.SupportedExtensions.Concat(ArchiveStreamProvider.SupportedExtensions).ToArray();
                SelectedGameFile = await FileSystem.PickFileAsync(extensions);
            }
            if (SelectedGameFile == null)
            {
                return;
            }

            await StartGameAsync(system, SelectedGameFile);
        }

        private async Task StartGameAsync(GameSystemViewModel system, IFileInfo file)
        {
            var dependenciesMet = await system.CheckDependenciesMetAsync();
            if (!dependenciesMet)
            {
                ResetSystemsSelection();
                await DialogsService.AlertAsync(Resources.Strings.SystemUnmetDependenciesAlertTitle, Resources.Strings.SystemUnmetDependenciesAlertMessage);
                return;
            }

            var folderNeeded = system.CheckRootFolderRequired(file);
            var folder = default(IDirectoryInfo);
            if (folderNeeded)
            {
                await DialogsService.AlertAsync(Resources.Strings.SelectFolderRequestAlertTitle, Resources.Strings.SelectFolderRequestAlertMessage);
                folder = await FileSystem.PickDirectoryAsync();
                if (folder == null)
                {
                    ResetSystemsSelection();
                    return;
                }

                if (!Path.GetDirectoryName(file.FullName).StartsWith(folder.FullName))
                {
                    ResetSystemsSelection();
                    await DialogsService.AlertAsync(Resources.Strings.SelectFolderInvalidAlertTitle, Resources.Strings.SelectFolderInvalidAlertMessage);
                    return;
                }
            }

            var param = await GenerateGameLaunchParamAsync(system, file, folder);
            var task = NavigationService.Navigate<GamePlayerViewModel, GamePlayerViewModel.Parameter>(param);
        }

        private void ResetSystemsSelection()
        {
            //Reset systems selection
            GameSystems = EmulationService.Systems;
            SelectedGameFile = null;
        }

        private async Task<GamePlayerViewModel.Parameter> GenerateGameLaunchParamAsync(GameSystemViewModel system, IFileInfo file, IDirectoryInfo rootFolder)
        {
            var vfsRomPath = "ROM";
            var vfsSystemPath = "System";
            var vfsSavePath = "Save";

            var core = system.Core;

            string virtualMainFilePath = null;
            var provider = default(IStreamProvider);

            if (core.NativeArchiveSupport || !ArchiveStreamProvider.SupportedExtensions.Contains(Path.GetExtension(file.Name)))
            {
                virtualMainFilePath = $"{vfsRomPath}{Path.DirectorySeparatorChar}{file.Name}";
                provider = new SingleFileStreamProvider(virtualMainFilePath, file);
                if (rootFolder != null)
                {
                    virtualMainFilePath = file.FullName.Substring(rootFolder.FullName.Length + 1);
                    virtualMainFilePath = $"{vfsRomPath}{Path.DirectorySeparatorChar}{virtualMainFilePath}";
                    provider = new FolderStreamProvider(vfsRomPath, rootFolder);
                }
            }
            else
            {
                var archiveProvider = new ArchiveStreamProvider(vfsRomPath, file);
                await archiveProvider.InitializeAsync();
                provider = archiveProvider;
                var entries = await provider.ListEntriesAsync();
                virtualMainFilePath = entries.FirstOrDefault(d => system.SupportedExtensions.Contains(Path.GetExtension(d)));
            }

            var systemFolder = await system.GetSystemDirectoryAsync();
            var systemProvider = new FolderStreamProvider(vfsSystemPath, systemFolder);
            core.SystemRootPath = vfsSystemPath;
            var saveFolder = await system.GetSaveDirectoryAsync();
            var saveProvider = new FolderStreamProvider(vfsSavePath, saveFolder);
            core.SaveRootPath = vfsSavePath;

            provider = new CombinedStreamProvider(new HashSet<IStreamProvider>() { provider, systemProvider, saveProvider });

            return new GamePlayerViewModel.Parameter(core, provider, virtualMainFilePath);
        }
    }
}