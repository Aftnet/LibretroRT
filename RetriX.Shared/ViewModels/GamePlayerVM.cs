﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RetriX.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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

            SaveStateSlot1 = new RelayCommand(() => SaveState(0), () => CoreOperationsAllowed);
            SaveStateSlot2 = new RelayCommand(() => SaveState(1), () => CoreOperationsAllowed);
            SaveStateSlot3 = new RelayCommand(() => SaveState(2), () => CoreOperationsAllowed);
            SaveStateSlot4 = new RelayCommand(() => SaveState(3), () => CoreOperationsAllowed);
            SaveStateSlot5 = new RelayCommand(() => SaveState(4), () => CoreOperationsAllowed);
            SaveStateSlot6 = new RelayCommand(() => SaveState(5), () => CoreOperationsAllowed);

            LoadStateSlot1 = new RelayCommand(() => LoadState(0), () => CoreOperationsAllowed);
            LoadStateSlot2 = new RelayCommand(() => LoadState(1), () => CoreOperationsAllowed);
            LoadStateSlot3 = new RelayCommand(() => LoadState(2), () => CoreOperationsAllowed);
            LoadStateSlot4 = new RelayCommand(() => LoadState(3), () => CoreOperationsAllowed);
            LoadStateSlot5 = new RelayCommand(() => LoadState(4), () => CoreOperationsAllowed);
            LoadStateSlot6 = new RelayCommand(() => LoadState(5), () => CoreOperationsAllowed);

            AllCoreCommands = new RelayCommand[] { TogglePauseCommand, ResetCommand,
                SaveStateSlot1, SaveStateSlot2, SaveStateSlot3, SaveStateSlot4, SaveStateSlot5, SaveStateSlot6,
                LoadStateSlot1, LoadStateSlot2, LoadStateSlot3, LoadStateSlot4, LoadStateSlot5, LoadStateSlot6
            };

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

        public void Activated()
        {
            CoreOperationsAllowed = true;
            PlatformService.HandleGameplayKeyShortcuts = true;
        }

        public void Deactivated()
        {
            CoreOperationsAllowed = false;
            PlatformService.HandleGameplayKeyShortcuts = false;
        }

        private void TogglePause()
        {
            EmulationService.GamePaused = !EmulationService.GamePaused;
        }

        private async void Reset()
        {
            CoreOperationsAllowed = false;
            await EmulationService.ResetGameAsync();
            CoreOperationsAllowed = true;
        }

        private async void SaveState(uint slotID)
        {
            CoreOperationsAllowed = false;

            var data = await EmulationService.SaveGameStateAsync();
            if (data != null)
            {
                SaveStateService.GameId = EmulationService.GameID;
                await SaveStateService.SaveStateAsync(slotID, data);
            }

            CoreOperationsAllowed = true;
        }

        private async void LoadState(uint slotID)
        {
            CoreOperationsAllowed = false;

            SaveStateService.GameId = EmulationService.GameID;
            var data = await SaveStateService.LoadStateAsync(slotID);
            if (data != null)
            {
                await EmulationService.LoadGameStateAsync(data);
            }

            CoreOperationsAllowed = true;
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
