using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RetriX.Shared.Services;
using System;
using System.Threading;

namespace RetriX.Shared.ViewModels
{
    public class GamePlayerVM : ViewModelBase
    {
        private static readonly TimeSpan UIInactivityCheckInterval = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan UIHidingTime = TimeSpan.FromSeconds(3);

        private readonly IPlatformService PlatformService;
        private readonly IEmulationService EmulationService;

        public RelayCommand TappedCommand { get; private set; }
        public RelayCommand PointerMovedCommand { get; private set; }
        public RelayCommand ToggleFullScreenCommand { get; private set; }
        public RelayCommand TogglePauseCommand { get; private set; }
        public RelayCommand ResetCommand { get; private set; }

        public bool IsFullScreenMode => PlatformService.IsFullScreenMode;
        public bool IsPaused => EmulationService.GamePaused;

        private bool displayPlayerUI;
        public bool DisplayPlayerUI
        {
            get { return displayPlayerUI; }
            set { Set(ref displayPlayerUI, value); }
        }

        private Timer PlayerUIInactivityTimer;
        private DateTimeOffset LastUIActivityTime = DateTimeOffset.UtcNow;

        public GamePlayerVM(IPlatformService platformService, IEmulationService emulationService)
        {
            PlatformService = platformService;
            EmulationService = emulationService;

            TappedCommand = new RelayCommand(ReactToUserUIActivity);
            PointerMovedCommand = new RelayCommand(ReactToUserUIActivity);
            ToggleFullScreenCommand = new RelayCommand(ToggleFullScreen);
            TogglePauseCommand = new RelayCommand(TogglePause);
            ResetCommand = new RelayCommand(Reset);

            EmulationService.GamePausedChanged += OnGamePausedChanged;

            PlayerUIInactivityTimer = new Timer(d => HideUIIfUserInactive(), null, UIInactivityCheckInterval, UIInactivityCheckInterval);
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
        }

        private void Reset()
        {
            EmulationService.ResetGame();
        }

        private void OnGamePausedChanged()
        {
            RaisePropertyChanged(nameof(IsPaused));
        }

        private void ReactToUserUIActivity()
        {
            DisplayPlayerUI = true;
            LastUIActivityTime = DateTimeOffset.UtcNow;
        }

        private void HideUIIfUserInactive()
        {
            if (DateTimeOffset.UtcNow.Subtract(LastUIActivityTime).CompareTo(UIHidingTime) >= 0)
            {
                PlatformService.RunOnUIThreadAsync(() => DisplayPlayerUI = false);
            }
        }
    }
}
