using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RetriX.Shared.Services;
using System;
using System.Collections;
using System.Linq;
using System.Threading;

namespace RetriX.Shared.ViewModels
{
    public class GamePlayerVM : ViewModelBase
    {
        private const int NumSaveSlots = 8;

        private static readonly TimeSpan UIInactivityCheckInterval = TimeSpan.FromSeconds(0.5);
        private static readonly TimeSpan UIHidingTime = TimeSpan.FromSeconds(3);

        private readonly IPlatformService PlatformService;
        private readonly IEmulationService EmulationService;
        private readonly ISaveStateService SaveStateService;

        public RelayCommand TappedCommand { get; private set; }
        public RelayCommand PointerMovedCommand { get; private set; }
        public RelayCommand ToggleFullScreenCommand { get; private set; }

        public RelayCommand TogglePauseCommand { get; private set; }
        public RelayCommand ResetCommand { get; private set; }

        public RelayCommand[] SaveStateCommands { get; private set; }
        public RelayCommand[] LoadStateCommands { get; private set; }

        private RelayCommand[] AllCoreCommands;

        private bool coreOperationsAllowed = false;
        public bool CoreOperationsAllowed
        {
            get { return coreOperationsAllowed; }
            set
            {
                if (Set(ref coreOperationsAllowed, value))
                {
                    foreach (var i in AllCoreCommands)
                    {
                        i.RaiseCanExecuteChanged();
                    }
                }
            }
        }

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

        public GamePlayerVM(IPlatformService platformService, IEmulationService emulationService, ISaveStateService saveStateService)
        {
            PlatformService = platformService;
            EmulationService = emulationService;
            SaveStateService = saveStateService;

            TappedCommand = new RelayCommand(ReactToUserUIActivity);
            PointerMovedCommand = new RelayCommand(ReactToUserUIActivity);
            ToggleFullScreenCommand = new RelayCommand(ToggleFullScreen);

            TogglePauseCommand = new RelayCommand(TogglePause, () => CoreOperationsAllowed);
            ResetCommand = new RelayCommand(Reset, () => CoreOperationsAllowed);

            const int saveSlotIDStart = 0;
            SaveStateCommands = Enumerable.Range(saveSlotIDStart, NumSaveSlots).Select(d => new RelayCommand(() => SaveState((uint)d), () => CoreOperationsAllowed)).ToArray();
            LoadStateCommands = Enumerable.Range(saveSlotIDStart, NumSaveSlots).Select(d => new RelayCommand(() => LoadState((uint)d), () => CoreOperationsAllowed)).ToArray();

            AllCoreCommands = SaveStateCommands.Concat(LoadStateCommands).Concat(new RelayCommand[] { TogglePauseCommand, ResetCommand }).ToArray();

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
            EmulationService.ResetGameAsync();
        }

        private async void SaveState(uint slotID)
        {
            var data = await EmulationService.SaveGameStateAsync();
            await SaveStateService.SaveStateAsync(slotID, data);
        }

        private async void LoadState(uint slotID)
        {
            var data = await SaveStateService.LoadStateAsync(slotID);
            if (data == null)
            {
                return;
            }

            await EmulationService.LoadGameStateAsync(data);
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
