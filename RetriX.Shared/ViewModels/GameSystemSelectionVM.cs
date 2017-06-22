using Acr.UserDialogs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PCLStorage;
using RetriX.Shared.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetriX.Shared.ViewModels
{
    public class GameSystemSelectionVM<T> : ViewModelBase where T : GameSystemVMBase
    {
        public const string SelectFolderRequestAlertTitleKey = nameof(SelectFolderRequestAlertTitleKey);
        public const string SelectFolderRequestAlertMessageKey = nameof(SelectFolderRequestAlertMessageKey);

        public const string GameLoadingFailAlertTitleKey = nameof(GameLoadingFailAlertTitleKey);
        public const string GameLoadingFailAlertMessageKey = nameof(GameLoadingFailAlertMessageKey);
        public const string GameRunningFailAlertTitleKey = nameof(GameRunningFailAlertTitleKey);
        public const string GameRunningFailAlertMessageKey = nameof(GameRunningFailAlertMessageKey);

        private readonly IUserDialogs DialogsService;
        private readonly ILocalizationService LocalizationService;
        private readonly IPlatformService PlatformService;
        private readonly IEmulationService<T> EmulationService;

        public IReadOnlyList<T> GameSystems => EmulationService.Systems;
        public RelayCommand<T> GameSystemSelectedCommand { get; private set; }

        public GameSystemSelectionVM(IUserDialogs dialogsService, ILocalizationService localizationService, IPlatformService platformService, IEmulationService<T> emulationService)
        {
            DialogsService = dialogsService;
            LocalizationService = localizationService;
            PlatformService = platformService;
            EmulationService = emulationService;

            GameSystemSelectedCommand = new RelayCommand<T>(GameSystemSelected);

            EmulationService.RequestGameFolderAsync = OnRequestGameFolderAsync;
            EmulationService.GameRuntimeExceptionOccurred += OnGameRuntimeExceptionOccurred;
        }

        public async void GameSystemSelected(T selectedSystem)
        {
            var extensions = selectedSystem.SupportedExtensions;
            var file = await PlatformService.SelectFileAsync(extensions);
            if (file == null)
            {
                return;
            }

            var result = await EmulationService.StartGameAsync(selectedSystem, file);
            if (!result)
            {
                await DisplayNotification(GameLoadingFailAlertTitleKey, GameLoadingFailAlertMessageKey);
            }
        }

        public async Task StartGameFromFileAsync(IFile file)
        {
            var result = await EmulationService.StartGameAsync(file);
            if (!result)
            {
                await DisplayNotification(GameLoadingFailAlertTitleKey, GameLoadingFailAlertMessageKey);
            }
        }

        private async Task<IFolder> OnRequestGameFolderAsync(IEmulationService sender)
        {
            await DisplayNotification(SelectFolderRequestAlertTitleKey, SelectFolderRequestAlertMessageKey);
            var folder = await PlatformService.SelectFolderAsync();
            return folder;
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