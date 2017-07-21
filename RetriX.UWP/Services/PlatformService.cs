using PCLStorage;
using RetriX.Shared.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Pickers;
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
            "Windows.Desktop", "Windows.Team"
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

        public event FullScreenChangeRequestedDelegate FullScreenChangeRequested;

        public event PauseToggleRequestedDelegate PauseToggleRequested;

        public event GameStateOperationRequestedDelegate GameStateOperationRequested;

        public bool ChangeFullScreenState(FullScreenChangeType changeType)
        {
            if ((changeType == FullScreenChangeType.Enter && IsFullScreenMode) || (changeType == FullScreenChangeType.Exit && !IsFullScreenMode))
            {
                return true;
            }

            if (changeType == FullScreenChangeType.Toggle)
            {
                changeType = IsFullScreenMode ? FullScreenChangeType.Exit : FullScreenChangeType.Enter;
            }

            switch (changeType)
            {
                case FullScreenChangeType.Enter:
                    return AppView.TryEnterFullScreenMode();
                case FullScreenChangeType.Exit:
                    AppView.ExitFullScreenMode();
                    return true;
            }

            throw new Exception("this should never happen");
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

        public async Task<IFile> SelectFileAsync(IEnumerable<string> extensionsFilter)
        {
            var picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            foreach (var i in extensionsFilter)
            {
                picker.FileTypeFilter.Add(i);
            }

            var file = await picker.PickSingleFileAsync();
            return file == null ? null : new WinRTFile(file);
        }

        public async Task<IFolder> SelectFolderAsync()
        {
            var picker = new FolderPicker();
            picker.FileTypeFilter.Add("*");
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;

            var folder = await picker.PickSingleFolderAsync();
            if (folder == null)
            {
                return null;
            }

            var accessList = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList;
            accessList.Clear();
            accessList.Add(folder);
            return new WinRTFolder(folder);
        }

        public void CopyToClipboard(string content)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(content);
            Clipboard.SetContent(dataPackage);
        }

        private void OnKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            var shiftState = sender.GetKeyState(VirtualKey.Shift);
            var shiftIsDown = (shiftState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;

            var altState = sender.GetKeyState(VirtualKey.LeftMenu);
            var altIsDown = (altState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;

            var gamepadViewState = sender.GetKeyState(VirtualKey.GamepadView);
            var gamepadViewIsDown = (gamepadViewState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;

            switch (args.VirtualKey)
            {
                //By default the gamepad's B button is treated as a hardware back button.
                //Handling the KeyDown event disables this.
                //We want this to happen only in the game page and not in the rest of the UI
                case VirtualKey.GamepadB:
                    args.Handled = true;
                    break;

                //Alt+Enter: enter fullscreen
                case VirtualKey.Enter:
                    if (shiftIsDown)
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
                    PauseToggleRequested(this);
                    args.Handled = true;
                    break;

                case VirtualKey.GamepadMenu:
                    if(gamepadViewIsDown)
                    {
                        PauseToggleRequested(this);
                        args.Handled = true;
                    }
                    break;

                case VirtualKey.F1:
                    HandleFunctionKeyPress(shiftIsDown, 1, args);
                    break;

                case VirtualKey.F2:
                    HandleFunctionKeyPress(shiftIsDown, 2, args);
                    break;

                case VirtualKey.F3:
                    HandleFunctionKeyPress(shiftIsDown, 3, args);
                    break;

                case VirtualKey.F4:
                    HandleFunctionKeyPress(shiftIsDown, 4, args);
                    break;

                case VirtualKey.F5:
                    HandleFunctionKeyPress(shiftIsDown, 5, args);
                    break;

                case VirtualKey.F6:
                    HandleFunctionKeyPress(shiftIsDown, 6, args);
                    break;
            }
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
