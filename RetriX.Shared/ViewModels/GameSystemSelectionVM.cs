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
                new GameSystemListItemVM{ Type = GameSystemTypes.NES, Name = localizationService.GetLocalizedString("SystemNameNES"), Symbol = "\uf118" },
                new GameSystemListItemVM{ Type = GameSystemTypes.SNES, Name = localizationService.GetLocalizedString("SystemNameSNES"), Symbol = "\uf119" },
                new GameSystemListItemVM{ Type = GameSystemTypes.GB, Name = localizationService.GetLocalizedString("SystemNameGameBoy"), Symbol = "\uf11b" },
                new GameSystemListItemVM{ Type = GameSystemTypes.GBA, Name = localizationService.GetLocalizedString("SystemNameGameBoyAdvance"), Symbol = "\uf115" },
                new GameSystemListItemVM{ Type = GameSystemTypes.MegaDrive, Name = localizationService.GetLocalizedString("SystemNameMegaDrive"), Symbol = "\uf124" },
            };

            GameSystemSelectedCommand = new RelayCommand<GameSystemListItemVM>(GameSystemSelected);
        }

        public void GameSystemSelected(GameSystemListItemVM selectedSystem)
        {
            EmulationService.SelectAndRunGameForSystem(selectedSystem.Type);
        }
    }
}