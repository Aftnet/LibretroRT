using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RetriX.Shared.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RetriX.Shared.ViewModels
{
    public class GamePlayerVM : ViewModelBase
    {
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
        public RelayCommand StopCommand { get; private set; }

        public RelayCommand SaveStateSlot1 { get; private set; }
        public RelayCommand SaveStateSlot2 { get; private set; }
        public RelayCommand SaveStateSlot3 { get; private set; }
        public RelayCommand SaveStateSlot4 { get; private set; }
        public RelayCommand SaveStateSlot5 { get; private set; }
        public RelayCommand SaveStateSlot6 { get; private set; }

        public RelayCommand LoadStateSlot1 { get; private set; }
        public RelayCommand LoadStateSlot2 { get; private set; }
        public RelayCommand LoadStateSlot3 { get; private set; }
        public RelayCommand LoadStateSlot4 { get; private set; }
        public RelayCommand LoadStateSlot5 { get; private set; }
        public RelayCommand LoadStateSlot6 { get; private set; }

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

        private bool gameIsPaused;
        public bool GameIsPaused
        {
            get { return gameIsPaused; }
            set { Set(ref gameIsPaused, value); }
        }

        private bool displayPlayerUI;
        public bool DisplayPlayerUI
        {
            get { return displayPlayerUI; }
            set
            {
                if (Set(ref displayPlayerUI, value))
                {
                    PlatformService.ChangeMousePointerVisibility(value? MousePointerVisibility.Visible : MousePointerVisibility.Hidden);
                }
            }
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
            ToggleFullScreenCommand = new RelayCommand(() => RequestFullScreenChange(FullScreenChangeType.Toggle));

            TogglePauseCommand = new RelayCommand(TogglePause, () => CoreOperationsAllowed);
            ResetCommand = new RelayCommand(Reset, () => CoreOperationsAllowed);
            StopCommand = new RelayCommand(Stop, () => CoreOperationsAllowed);

            SaveStateSlot1 = new RelayCommand(() => SaveState(1), () => CoreOperationsAllowed);
            SaveStateSlot2 = new RelayCommand(() => SaveState(2), () => CoreOperationsAllowed);
            SaveStateSlot3 = new RelayCommand(() => SaveState(3), () => CoreOperationsAllowed);
            SaveStateSlot4 = new RelayCommand(() => SaveState(4), () => CoreOperationsAllowed);
            SaveStateSlot5 = new RelayCommand(() => SaveState(5), () => CoreOperationsAllowed);
            SaveStateSlot6 = new RelayCommand(() => SaveState(6), () => CoreOperationsAllowed);

            LoadStateSlot1 = new RelayCommand(() => LoadState(1), () => CoreOperationsAllowed);
            LoadStateSlot2 = new RelayCommand(() => LoadState(2), () => CoreOperationsAllowed);
            LoadStateSlot3 = new RelayCommand(() => LoadState(3), () => CoreOperationsAllowed);
            LoadStateSlot4 = new RelayCommand(() => LoadState(4), () => CoreOperationsAllowed);
            LoadStateSlot5 = new RelayCommand(() => LoadState(5), () => CoreOperationsAllowed);
            LoadStateSlot6 = new RelayCommand(() => LoadState(6), () => CoreOperationsAllowed);

            AllCoreCommands = new RelayCommand[] { TogglePauseCommand, ResetCommand, StopCommand,
                SaveStateSlot1, SaveStateSlot2, SaveStateSlot3, SaveStateSlot4, SaveStateSlot5, SaveStateSlot6,
                LoadStateSlot1, LoadStateSlot2, LoadStateSlot3, LoadStateSlot4, LoadStateSlot5, LoadStateSlot6
            };

            EmulationService.GameStarted += OnGameStarted;
            PlatformService.FullScreenChangeRequested += (d, e) => RequestFullScreenChange(e.Type);
            PlatformService.GameStateOperationRequested += OnGameStateOperationRequested;
        }

        private async void RequestFullScreenChange(FullScreenChangeType fullScreenChangeType)
        {
            PlatformService.ChangeFullScreenState(fullScreenChangeType);

            //Fullscreen toggling takes some time
            await Task.Delay(100);
            RaisePropertyChanged(nameof(IsFullScreenMode));
        }

        public void Activated()
        {
            CoreOperationsAllowed = true;
            PlatformService.HandleGameplayKeyShortcuts = true;
            PlayerUIInactivityTimer = new Timer(d => HideUIIfUserInactive(), null, UIInactivityCheckInterval, UIInactivityCheckInterval);
        }

        public void Deactivated()
        {
            PlayerUIInactivityTimer.Dispose();
            CoreOperationsAllowed = false;
            PlatformService.HandleGameplayKeyShortcuts = false;
            DisplayPlayerUI = true;
        }

        private async void TogglePause()
        {
            CoreOperationsAllowed = false;

            if (GameIsPaused)
            {
                await EmulationService.ResumeGameAsync();
            }
            else
            {
                await EmulationService.PauseGameAsync();
            }

            GameIsPaused = !GameIsPaused;
            CoreOperationsAllowed = true;
            ReactToUserUIActivity();
        }

        private async void Reset()
        {
            CoreOperationsAllowed = false;
            await EmulationService.ResetGameAsync();
            CoreOperationsAllowed = true;
        }

        private async void Stop()
        {
            CoreOperationsAllowed = false;
            await EmulationService.StopGameAsync();
            CoreOperationsAllowed = true;
        }

        private async void SaveState(uint slotID)
        {
            CoreOperationsAllowed = false;

            var data = await EmulationService.SaveGameStateAsync();
            if (data != null)
            {
                SaveStateService.SetGameId(EmulationService.GameID);
                await SaveStateService.SaveStateAsync(slotID, data);
            }

            CoreOperationsAllowed = true;
        }

        private async void LoadState(uint slotID)
        {
            CoreOperationsAllowed = false;

            SaveStateService.SetGameId(EmulationService.GameID);
            var data = await SaveStateService.LoadStateAsync(slotID);
            if (data != null)
            {
                await EmulationService.LoadGameStateAsync(data);
            }

            CoreOperationsAllowed = true;
        }

        private void OnGameStarted(IEmulationService sender)
        {
            GameIsPaused = false;
        }

        private void OnGameStateOperationRequested(IPlatformService sender, GameStateOperationEventArgs args)
        {
            if (!CoreOperationsAllowed)
            {
                return;
            }

            if (args.Type == GameStateOperationEventArgs.GameStateOperationType.Load)
            {
                LoadState(args.SlotID);
            }
            else
            {
                SaveState(args.SlotID);
            }
        }

        private void ReactToUserUIActivity()
        {
            DisplayPlayerUI = true;
            LastUIActivityTime = DateTimeOffset.UtcNow;
        }

        private void HideUIIfUserInactive()
        {
            if (GameIsPaused)
            {
                return;
            }

            if (DateTimeOffset.UtcNow.Subtract(LastUIActivityTime).CompareTo(UIHidingTime) >= 0)
            {
                PlatformService.RunOnUIThreadAsync(() => DisplayPlayerUI = false);
            }
        }
    }
}
