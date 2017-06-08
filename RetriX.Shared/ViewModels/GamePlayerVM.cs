using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RetriX.Shared.Services;

namespace RetriX.Shared.ViewModels
{
    public class GamePlayerVM : ViewModelBase
    {
        private readonly IEmulationService EmulationService;

        public RelayCommand ToggleFullScreenCommand { get; private set; }
        public bool IsFullScreenMode => EmulationService.IsFullScreenMode;

        public GamePlayerVM(IEmulationService emulationService)
        {
            EmulationService = emulationService;

            ToggleFullScreenCommand = new RelayCommand(ToggleFullScreen);
        }

        private void ToggleFullScreen()
        {
            if (EmulationService.IsFullScreenMode)
            {
                EmulationService.ExitFullScreen();
            }
            else
            {
                EmulationService.TryEnterFullScreen();
            }

            RaisePropertyChanged(nameof(IsFullScreenMode));
        }
    }
}
