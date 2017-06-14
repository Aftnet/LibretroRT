﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RetriX.Shared.Services;
using System.Collections.Generic;

namespace RetriX.Shared.ViewModels
{
    public class GameSystemSelectionVM : ViewModelBase
    {
        private readonly IPlatformService PlatformService;
        private readonly IEmulationService EmulationService;

        private readonly IReadOnlyList<GameSystemListItemVM> gameSystems;
        public IReadOnlyList<GameSystemListItemVM> GameSystems => gameSystems;

        public RelayCommand<GameSystemListItemVM> GameSystemSelectedCommand { get; private set; }

        public GameSystemSelectionVM(ILocalizationService localizationService, IPlatformService platformService, IEmulationService emulationService)
        {
            PlatformService = platformService;
            EmulationService = emulationService;

            gameSystems = new GameSystemListItemVM[]
            {
                new GameSystemListItemVM(localizationService, GameSystemTypes.NES, "SystemNameNES", "ManufacturerNameNintendo", "\uf118"),
                new GameSystemListItemVM(localizationService, GameSystemTypes.SNES, "SystemNameSNES", "ManufacturerNameNintendo", "\uf119"),
                new GameSystemListItemVM(localizationService, GameSystemTypes.GB, "SystemNameGameBoy", "ManufacturerNameNintendo", "\uf11b"),
                new GameSystemListItemVM(localizationService, GameSystemTypes.GBA, "SystemNameGameBoyAdvance", "ManufacturerNameNintendo", "\uf115"),
                new GameSystemListItemVM(localizationService, GameSystemTypes.MegaDrive, "SystemNameMegaDrive", "ManufacturerNameSega", "\uf124"),
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

            await EmulationService.StartGameAsync(systemType, file);
        }
    }
}