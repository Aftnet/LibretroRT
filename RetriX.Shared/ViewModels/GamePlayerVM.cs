using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RetriX.Shared.Services;

namespace RetriX.Shared.ViewModels
{
    public class GamePlayerVM : ViewModelBase
    {
        private readonly IPlatformService PlatformService;

        private readonly IEmulationService EmulationService;

        public RelayCommand ToggleFullScreenCommand { get; private set; }
        public bool IsFullScreenMode => PlatformService.IsFullScreenMode;

        public GamePlayerVM(IPlatformService platformService, IEmulationService emulationService)
        {
            PlatformService = platformService;
            EmulationService = emulationService;

            ToggleFullScreenCommand = new RelayCommand(ToggleFullScreen);
        }

        private void ToggleFullScreen()
        {
            if (PlatformService.IsFullScreenMode)
            {
                PlatformService.ExitFullScreen();
            }
            else
            {
                PlatformService.TryEnterFullScreen();
            }

            RaisePropertyChanged(nameof(IsFullScreenMode));
        }
    }
}
