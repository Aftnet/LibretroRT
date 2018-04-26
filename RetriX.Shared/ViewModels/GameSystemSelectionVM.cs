using Acr.UserDialogs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Plugin.FileSystem.Abstractions;
using RetriX.Shared.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RetriX.Shared.ViewModels
{
    public class GameSystemSelectionVM : ViewModelBase
    {
        private readonly IFileSystem FileSystem;
        private readonly IUserDialogs DialogsService;
        private readonly IPlatformService PlatformService;
        private readonly IEmulationService EmulationService;

        private IFileInfo SelectedGameFile;

        private IReadOnlyList<GameSystemVM> gameSystems;
        public IReadOnlyList<GameSystemVM> GameSystems
        {
            get { return gameSystems; }
            private set { Set(ref gameSystems, value); }
        }

        public RelayCommand<GameSystemVM> GameSystemSelectedCommand { get; private set; }

        public GameSystemSelectionVM(IFileSystem fileSystem, IUserDialogs dialogsService, IPlatformService platformService, IEmulationService emulationService)
        {
            FileSystem = fileSystem;
            DialogsService = dialogsService;
            PlatformService = platformService;
            EmulationService = emulationService;

            GameSystems = EmulationService.Systems;
            GameSystemSelectedCommand = new RelayCommand<GameSystemVM>(GameSystemSelected);

            EmulationService.GameRuntimeExceptionOccurred += OnGameRuntimeExceptionOccurred;
            EmulationService.GameStopped += d => { ResetSystemsSelection(); };

            ResetSystemsSelection();
        }

        public async void GameSystemSelected(GameSystemVM system)
        {
            if (SelectedGameFile == null)
            {
                var extensions = system.SupportedExtensions.Concat(EmulationService.ArchiveExtensions).ToArray();
                SelectedGameFile = await FileSystem.PickFileAsync(extensions);
            }
            if (SelectedGameFile == null)
            {
                return;
            }

            await StartGameAsync(system, SelectedGameFile);
        }

        public async Task StartGameFromFileAsync(IFileInfo file)
        {
            //Find compatible systems for file extension
            var extension = Path.GetExtension(file.Name);
            var compatibleSystems = EmulationService.Systems.Where(d => d.SupportedExtensions.Contains(extension));

            //If none, do nothing
            if (!compatibleSystems.Any())
            {
                return;
            }

            //If just one, start game with it
            if (compatibleSystems.Count() == 1)
            {
                await StartGameAsync(compatibleSystems.First(), file);
                return;
            }

            //If multiple ones, filter system selection accordingly and have user select a system
            await EmulationService.StopGameAsync();
            GameSystems = compatibleSystems.ToArray();
            SelectedGameFile = file;
        }

        private async Task StartGameAsync(GameSystemVM system, IFileInfo file)
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

            var startSuccess = await EmulationService.StartGameAsync(system, file, folder);
            if (!startSuccess)
            {
                ResetSystemsSelection();
                await DialogsService.AlertAsync(Resources.Strings.GameLoadingFailAlertTitle, Resources.Strings.GameLoadingFailAlertMessage);
            }
        }

        private void OnGameRuntimeExceptionOccurred(IEmulationService sender, Exception e)
        {
            ResetSystemsSelection();
            DialogsService.AlertAsync(Resources.Strings.GameRunningFailAlertTitle, Resources.Strings.GameRunningFailAlertMessage);
        }

        private void ResetSystemsSelection()
        {
            //Reset systems selection
            GameSystems = EmulationService.Systems;
            SelectedGameFile = null;
        }
    }
}