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
                new GameSystemListItemVM{ Type = GameSystemTypes.NES, Name = localizationService.GetLocalizedString("SystemNameNES"), Symbol = "\ue928" },
                new GameSystemListItemVM{ Type = GameSystemTypes.SNES, Name = localizationService.GetLocalizedString("SystemNameSNES"), Symbol = "\ue90f" },
                new GameSystemListItemVM{ Type = GameSystemTypes.GB, Name = localizationService.GetLocalizedString("SystemNameGameBoy"), Symbol = "\ue90d" },
                new GameSystemListItemVM{ Type = GameSystemTypes.GBA, Name = localizationService.GetLocalizedString("SystemNameGameBoyAdvance"), Symbol = "\ue90c" },
                new GameSystemListItemVM{ Type = GameSystemTypes.MegaDrive, Name = localizationService.GetLocalizedString("SystemNameMegaDrive"), Symbol = "\ue91f" },
            };

            GameSystemSelectedCommand = new RelayCommand<GameSystemListItemVM>(GameSystemSelected);
        }

        public void GameSystemSelected(GameSystemListItemVM selectedSystem)
        {
            EmulationService.SelectAndRunGameForSystem(selectedSystem.Type);
        }
    }
}