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
        public const string SelectFolderRequestAlertTitleKey = nameof(SelectFolderRequestAlertTitleKey);
        public const string SelectFolderRequestAlertMessageKey = nameof(SelectFolderRequestAlertMessageKey);
        public const string SelectFolderInvalidAlertTitleKey = nameof(SelectFolderInvalidAlertTitleKey);
        public const string SelectFolderInvalidAlertMessageKey = nameof(SelectFolderInvalidAlertMessageKey);

        public const string GameLoadingFailAlertTitleKey = nameof(GameLoadingFailAlertTitleKey);
        public const string GameLoadingFailAlertMessageKey = nameof(GameLoadingFailAlertMessageKey);
        public const string GameRunningFailAlertTitleKey = nameof(GameRunningFailAlertTitleKey);
        public const string GameRunningFailAlertMessageKey = nameof(GameRunningFailAlertMessageKey);
        public const string SystemUnmetDependenciesAlertTitleKey = nameof(SystemUnmetDependenciesAlertTitleKey);
        public const string SystemUnmetDependenciesAlertMessageKey = nameof(SystemUnmetDependenciesAlertMessageKey);

        private readonly IFileSystem FileSystem;
        private readonly IUserDialogs DialogsService;
        private readonly ILocalizationService LocalizationService;
        private readonly IPlatformService PlatformService;
        private readonly IEmulationService EmulationService;

        public IReadOnlyList<GameSystemVM> GameSystems => EmulationService.Systems;
        public RelayCommand<GameSystemVM> GameSystemSelectedCommand { get; private set; }

        public GameSystemSelectionVM(IFileSystem fileSystem, IUserDialogs dialogsService, ILocalizationService localizationService, IPlatformService platformService, IEmulationService emulationService)
        {
            FileSystem = fileSystem;
            DialogsService = dialogsService;
            LocalizationService = localizationService;
            PlatformService = platformService;
            EmulationService = emulationService;

            GameSystemSelectedCommand = new RelayCommand<GameSystemVM>(GameSystemSelected);

            EmulationService.CoresInitialized += OnCoresInitialized;
            EmulationService.GameRuntimeExceptionOccurred += OnGameRuntimeExceptionOccurred;
        }

        public async void GameSystemSelected(GameSystemVM system)
        {
            var extensions = system.SupportedExtensions.Concat(EmulationService.ArchiveExtensions).ToArray();
            var file = await FileSystem.PickFileAsync(extensions);
            if (file == null)
            {
                return;
            }

            await StartGameAsync(system, file);
        }

        public async Task StartGameFromFileAsync(IFileInfo file)
        {
            var system = await EmulationService.SuggestSystemForFileAsync(file);
            if (system == null)
            {
                return;
            }

            await StartGameAsync(system, file);
        }

        private async Task StartGameAsync(GameSystemVM system, IFileInfo file)
        {
            var dependenciesMet = await system.CheckDependenciesMetAsync();
            if (!dependenciesMet)
            {
                await DisplayNotification(SystemUnmetDependenciesAlertTitleKey, SystemUnmetDependenciesAlertMessageKey);
                return;
            }

            var folderNeeded = system.CheckRootFolderRequired(file);
            IDirectoryInfo folder = null;
            if (folderNeeded)
            {
                await DisplayNotification(SelectFolderRequestAlertTitleKey, SelectFolderRequestAlertMessageKey);
                folder = await FileSystem.PickDirectoryAsync();
                if (folder == null)
                {
                    return;
                }

                if (!Path.GetDirectoryName(file.FullName).StartsWith(folder.FullName))
                {
                    await DisplayNotification(SelectFolderInvalidAlertTitleKey, SelectFolderInvalidAlertMessageKey);
                    return;
                }
            }

            var startSuccess = await EmulationService.StartGameAsync(system, file, folder);
            if (!startSuccess)
            {
                await DisplayNotification(GameLoadingFailAlertTitleKey, GameLoadingFailAlertMessageKey);
            }
        }

        private void OnCoresInitialized(IEmulationService sender)
        {
            RaisePropertyChanged(nameof(GameSystems));
        }

        private void OnGameRuntimeExceptionOccurred(IEmulationService sender, Exception e)
        {
            DisplayNotification(GameRunningFailAlertTitleKey, GameRunningFailAlertMessageKey);
        }

        private Task DisplayNotification(string titleKey, string messageKey)
        {
            var title = LocalizationService.GetLocalizedString(titleKey);
            var message = LocalizationService.GetLocalizedString(messageKey);
            return DialogsService.AlertAsync(message, title);
        }
    }
}