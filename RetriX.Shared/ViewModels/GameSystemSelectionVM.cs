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
    public class GameSystemSelectionVM : ViewModelBase
    {
        public const string GameLoadingFailAlertTitleKey = nameof(GameLoadingFailAlertTitleKey);
        public const string GameLoadingFailAlertMessageKey = nameof(GameLoadingFailAlertMessageKey);
        public const string GameRunningFailAlertTitleKey = nameof(GameRunningFailAlertTitleKey);
        public const string GameRunningFailAlertMessageKey = nameof(GameRunningFailAlertMessageKey);

        private readonly IUserDialogs DialogsService;
        private readonly ILocalizationService LocalizationService;
        private readonly IPlatformService PlatformService;
        private readonly IEmulationService EmulationService;

        private readonly IReadOnlyList<GameSystemListItemVM> gameSystems;
        public IReadOnlyList<GameSystemListItemVM> GameSystems => gameSystems;

        public RelayCommand<GameSystemListItemVM> GameSystemSelectedCommand { get; private set; }

        public GameSystemSelectionVM(IUserDialogs dialogsService, ILocalizationService localizationService, IPlatformService platformService, IEmulationService emulationService)
        {
            DialogsService = dialogsService;
            LocalizationService = localizationService;
            PlatformService = platformService;
            EmulationService = emulationService;

            gameSystems = new GameSystemListItemVM[]
            {
                new GameSystemListItemVM(LocalizationService, GameSystemTypes.NES, "SystemNameNES", "ManufacturerNameNintendo", "\uf118"),
                new GameSystemListItemVM(LocalizationService, GameSystemTypes.SNES, "SystemNameSNES", "ManufacturerNameNintendo", "\uf119"),
                new GameSystemListItemVM(LocalizationService, GameSystemTypes.GB, "SystemNameGameBoy", "ManufacturerNameNintendo", "\uf11b"),
                new GameSystemListItemVM(LocalizationService, GameSystemTypes.GBA, "SystemNameGameBoyAdvance", "ManufacturerNameNintendo", "\uf115"),
                new GameSystemListItemVM(LocalizationService, GameSystemTypes.SG1000, "SystemNameSG1000", "ManufacturerNameSega", "\uf102", new string[]{ ".sg" }),
                new GameSystemListItemVM(LocalizationService, GameSystemTypes.MasterSystem, "SystemNameMasterSystem", "ManufacturerNameSega", "\uf118", new string[]{ ".sms" }),
                new GameSystemListItemVM(LocalizationService, GameSystemTypes.GameGear, "SystemNameGameGear", "ManufacturerNameSega", "\uf129", new string[]{ ".gg" }),
                new GameSystemListItemVM(LocalizationService, GameSystemTypes.MegaDrive, "SystemNameMegaDrive", "ManufacturerNameSega", "\uf124", new string[]{ ".mds", ".md", ".smd", ".gen" }),
            };

            GameSystemSelectedCommand = new RelayCommand<GameSystemListItemVM>(GameSystemSelected);
        }

        public async void GameSystemSelected(GameSystemListItemVM selectedSystem)
        {
            var systemType = selectedSystem.Type;
            var extensions = selectedSystem.SupportedExtensionsOverride;
            if (extensions == null)
            {
                extensions = EmulationService.GetSupportedExtensions(systemType);
            }

            var file = await PlatformService.SelectFileAsync(extensions);
            if (file == null)
            {
                return;
            }

            var result = await EmulationService.StartGameAsync(systemType, file);
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

        private void OnExceptionOccurred(IEmulationService sender, Exception e)
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