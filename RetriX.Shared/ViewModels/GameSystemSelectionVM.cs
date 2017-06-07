using GalaSoft.MvvmLight;
using RetriX.Shared.Services;
using System.Collections.Generic;

namespace RetriX.Shared.ViewModels
{
    public class GameSystemSelectionVM : ViewModelBase
    {
        private readonly IEmulationService EmulationService;

        public IReadOnlyList<GameSystemListItemVM> GameSystems => GameSystemListItemVM.Systems;

        public GameSystemSelectionVM(IEmulationService emulationService)
        {
            EmulationService = emulationService;
        }
    }
}
