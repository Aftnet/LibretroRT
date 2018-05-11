using RetriX.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Input;
using Windows.Gaming.Input;
using Windows.System;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace RetriX.UWP.Services
{
    public class PlatformService : IPlatformService
    {
        private static readonly ISet<string> DeviceFamiliesAllowingFullScreenChange = new HashSet<string>
        {
            "Windows.Desktop", "Windows.Team", "Windows.Mobile"
        };

        private ApplicationView AppView => ApplicationView.GetForCurrentView();
        private CoreWindow CoreWindow => CoreWindow.GetForCurrentThread();

        public bool FullScreenChangingPossible
        {
            get
            {
                var output = DeviceFamiliesAllowingFullScreenChange.Contains(AnalyticsInfo.VersionInfo.DeviceFamily);
                return output;
            }
        }

        public bool IsFullScreenMode => AppView.IsFullScreenMode;

        public bool ShouldDisplayTouchGamepad
        {
            get
            {
                var touchCapabilities = new TouchCapabilities();
                if (touchCapabilities.TouchPresent == 0)
                {
                    return false;
                }

                var keyboardCapabilities = new KeyboardCapabilities();
                if (keyboardCapabilities.KeyboardPresent != 0)
                {
                    return false;
                }

                if (Gamepad.Gamepads.Any())
                {
                    return false;
                }

                return true;
            }
        }

        private bool handleGameplayKeyShortcuts = false;
        public bool HandleGameplayKeyShortcuts
        {
            get { return handleGameplayKeyShortcuts; }
            set
            {
                handleGameplayKeyShortcuts = value;
                var window = CoreWindow.GetForCurrentThread();
                window.KeyDown -= OnKeyDown;
                if (handleGameplayKeyShortcuts)
                {
                    window.KeyDown += OnKeyDown;
                }
            }
        }

        public event EventHandler<FullScreenChangeEventArgs> FullScreenChangeRequested;

        public event EventHandler PauseToggleRequested;

        public event EventHandler<GameStateOperationEventArgs> GameStateOperationRequested;

        public async Task<bool> ChangeFullScreenStateAsync(FullScreenChangeType changeType)
        {
            if ((changeType == FullScreenChangeType.Enter && IsFullScreenMode) || (changeType == FullScreenChangeType.Exit && !IsFullScreenMode))
            {
                return true;
            }

            if (changeType == FullScreenChangeType.Toggle)
            {
                changeType = IsFullScreenMode ? FullScreenChangeType.Exit : FullScreenChangeType.Enter;
            }

            var result = false;
            switch (changeType)
            {
                case FullScreenChangeType.Enter:
                    result = AppView.TryEnterFullScreenMode();
                    break;
                case FullScreenChangeType.Exit:
                    AppView.ExitFullScreenMode();
                    result = true;
                    break;
                default:
                    throw new Exception("this should never happen");
            }

            await Task.Delay(100);
            return result;
        }

        public void ChangeMousePointerVisibility(MousePointerVisibility visibility)
        {
            var pointer = visibility == MousePointerVisibility.Hidden ? null : new CoreCursor(CoreCursorType.Arrow, 0);
            Window.Current.CoreWindow.PointerCursor = pointer;
        }

        public void ForceUIElementFocus()
        {
            FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
        }

        public void CopyToClipboard(string content)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(content);
            Clipboard.SetContent(dataPackage);
        }

        private void OnKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                //By default the gamepad's B button is treated as a hardware back button.
                //Handling the KeyDown event disables this.
                //We want this to happen only in the game page and not in the rest of the UI
                case VirtualKey.GamepadB:
                    args.Handled = true;
                    break;

                //Shift+Enter: enter fullscreen
                case VirtualKey.Enter:
                    if (KeyIsDown(sender, VirtualKey.Shift))
                    {
                        FullScreenChangeRequested(this, new FullScreenChangeEventArgs(FullScreenChangeType.Toggle));
                        args.Handled = true;
                    }
                    break;

                case VirtualKey.Shift:
                    if (KeyIsDown(sender, VirtualKey.Enter))
                    {
                        FullScreenChangeRequested(this, new FullScreenChangeEventArgs(FullScreenChangeType.Toggle));
                        args.Handled = true;
                    }
                    break;

                case VirtualKey.Escape:
                    FullScreenChangeRequested(this, new FullScreenChangeEventArgs(FullScreenChangeType.Exit));
                    args.Handled = true;
                    break;

                case VirtualKey.Space:
                    PauseToggleRequested(this, EventArgs.Empty);
                    args.Handled = true;
                    break;

                case VirtualKey.GamepadMenu:
                    if (KeyIsDown(sender, VirtualKey.GamepadView))
                    {
                        PauseToggleRequested(this, EventArgs.Empty);
                        args.Handled = true;
                    }
                    break;

                case VirtualKey.GamepadView:
                    if (KeyIsDown(sender, VirtualKey.GamepadMenu))
                    {
                        PauseToggleRequested(this, EventArgs.Empty);
                        args.Handled = true;
                    }
                    break;

                case VirtualKey.F1:
                    HandleFunctionKeyPress(KeyIsDown(sender, VirtualKey.Shift), 1, args);
                    break;

                case VirtualKey.F2:
                    HandleFunctionKeyPress(KeyIsDown(sender, VirtualKey.Shift), 2, args);
                    break;

                case VirtualKey.F3:
                    HandleFunctionKeyPress(KeyIsDown(sender, VirtualKey.Shift), 3, args);
                    break;

                case VirtualKey.F4:
                    HandleFunctionKeyPress(KeyIsDown(sender, VirtualKey.Shift), 4, args);
                    break;

                case VirtualKey.F5:
                    HandleFunctionKeyPress(KeyIsDown(sender, VirtualKey.Shift), 5, args);
                    break;

                case VirtualKey.F6:
                    HandleFunctionKeyPress(KeyIsDown(sender, VirtualKey.Shift), 6, args);
                    break;
            }
        }

        private bool KeyIsDown(CoreWindow window, VirtualKey key)
        {
            var keystate = window.GetKeyState(key);
            var output = (keystate & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
            return output;
        }

        public Task RunOnUIThreadAsync(Action action)
        {
            return CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action()).AsTask();
        }

        private void HandleFunctionKeyPress(bool shiftIsDown, uint slotID, KeyEventArgs args)
        {
            var eventArgs = new GameStateOperationEventArgs(shiftIsDown ? GameStateOperationEventArgs.GameStateOperationType.Save : GameStateOperationEventArgs.GameStateOperationType.Load, slotID);
            GameStateOperationRequested(this, eventArgs);

            args.Handled = true;
        }
    }
}
