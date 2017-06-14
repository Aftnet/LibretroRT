using Acr.UserDialogs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RetriX.Shared.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetriX.Shared.ViewModels
{
    public class GameSystemSelectionVM : ViewModelBase
    {
        public const string GameLoadingFailAlertTitleKey = "GameLoadingFailAlertTitleKey";
        public const string GameLoadingFailAlertMessageKey = "GameLoadingFailAlertMessageKey";

        private readonly ILocalizationService LocalizationService;
        private readonly IPlatformService PlatformService;
        private readonly IUserDialogs DialogsService;
        private readonly IEmulationService EmulationService;

        private readonly IReadOnlyList<GameSystemListItemVM> gameSystems;
        public IReadOnlyList<GameSystemListItemVM> GameSystems => gameSystems;

        public RelayCommand<GameSystemListItemVM> GameSystemSelectedCommand { get; private set; }

        public GameSystemSelectionVM(ILocalizationService localizationService, IPlatformService platformService, IUserDialogs dialogsService, IEmulationService emulationService)
        {
            LocalizationService = localizationService;
            PlatformService = platformService;
            DialogsService = dialogsService;
            EmulationService = emulationService;

            gameSystems = new GameSystemListItemVM[]
            {
                new GameSystemListItemVM(LocalizationService, GameSystemTypes.NES, "SystemNameNES", "ManufacturerNameNintendo", "\uf118"),
                new GameSystemListItemVM(LocalizationService, GameSystemTypes.SNES, "SystemNameSNES", "ManufacturerNameNintendo", "\uf119"),
                new GameSystemListItemVM(LocalizationService, GameSystemTypes.GB, "SystemNameGameBoy", "ManufacturerNameNintendo", "\uf11b"),
                new GameSystemListItemVM(LocalizationService, GameSystemTypes.GBA, "SystemNameGameBoyAdvance", "ManufacturerNameNintendo", "\uf115"),
                new GameSystemListItemVM(LocalizationService, GameSystemTypes.MegaDrive, "SystemNameMegaDrive", "ManufacturerNameSega", "\uf124"),
            };

            GameSystemSelectedCommand = new RelayCommand<GameSystemListItemVM>(GameSystemSelected);
        }

        public async void GameSystemSelected(GameSystemListItemVM selectedSystem)
        {
            var systemType = selectedSystem.Type;
            var extensions = EmulationService.GetSupportedExtensions(systemType);
            var file = await PlatformService.SelectFileAsync(extensions);
            if (file == null)
                return;

            var result = await EmulationService.StartGameAsync(systemType, file);
            if (!result)
            {
                await DisplayGameLoadingError();
            }
        }

        public async Task StartGameFromFileAsync(IPlatformFileWrapper file)
        {
            var result = await EmulationService.StartGameAsync(file);
            if (!result)
            {
                await DisplayGameLoadingError();
            }
        }

        private Task DisplayGameLoadingError()
        {
            var title = LocalizationService.GetLocalizedString(GameLoadingFailAlertTitleKey);
            var message = LocalizationService.GetLocalizedString(GameLoadingFailAlertMessageKey);
            return DialogsService.AlertAsync(message, title);
        }
    }
}