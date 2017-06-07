using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RetriX.Shared.Services;
using System.Collections.Generic;

namespace RetriX.Shared.ViewModels
{
    public class GameSystemSelectionVM : ViewModelBase
    {
        private readonly IEmulationService EmulationService;

        private readonly IReadOnlyList<GameSystemListItemVM> gameSystems;
        public IReadOnlyList<GameSystemListItemVM> GameSystems => gameSystems;

        public RelayCommand<GameSystemListItemVM> GameSystemSelectedCommand { get; private set; }

        public GameSystemSelectionVM(ILocalizationService localizationService, IEmulationService emulationService)
        {
            EmulationService = emulationService;

            gameSystems = new GameSystemListItemVM[]
            {
                new GameSystemListItemVM(GameSystemTypes.NES, localizationService.GetLocalizedString("SystemNameNES"), "\ue928"),
                new GameSystemListItemVM(GameSystemTypes.SNES, localizationService.GetLocalizedString("SystemNameSNES"), "\ue90f"),
                new GameSystemListItemVM(GameSystemTypes.GB, localizationService.GetLocalizedString("SystemNameGameBoy"), "\ue90d"),
                new GameSystemListItemVM(GameSystemTypes.GBA, localizationService.GetLocalizedString("SystemNameGameBoyAdvance"), "\ue90c"),
                new GameSystemListItemVM(GameSystemTypes.MegaDrive, localizationService.GetLocalizedString("SystemNameMegaDrive"), "\ue91f"),
            };

            GameSystemSelectedCommand = new RelayCommand<GameSystemListItemVM>(GameSystemSelected);
        }

        public void GameSystemSelected(GameSystemListItemVM selectedSystem)
        {
            EmulationService.SelectAndRunGame(selectedSystem.Type);
        }
    }
}