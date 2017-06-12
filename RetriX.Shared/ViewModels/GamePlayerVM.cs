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
        public RelayCommand TogglePauseCommand { get; private set; }
        public RelayCommand ResetCommand { get; private set; }

        public bool IsFullScreenMode => PlatformService.IsFullScreenMode;
        public bool IsPaused => EmulationService.GamePaused;

        public GamePlayerVM(IPlatformService platformService, IEmulationService emulationService)
        {
            PlatformService = platformService;
            EmulationService = emulationService;

            ToggleFullScreenCommand = new RelayCommand(ToggleFullScreen);
            TogglePauseCommand = new RelayCommand(TogglePause);
            ResetCommand = new RelayCommand(Reset);
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

        private void TogglePause()
        {
            EmulationService.GamePaused = !EmulationService.GamePaused;
            RaisePropertyChanged(nameof(IsPaused));
        }

        private void Reset()
        {
            EmulationService.ResetGame();
        }
    }
}
