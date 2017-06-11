using RetriX.Shared.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using System;

namespace RetriX.UWP.Services
{
    public class PlatformService : IPlatformService
    {
        private ApplicationView AppView => ApplicationView.GetForCurrentView();
        private CoreWindow CoreWindow => CoreWindow.GetForCurrentThread();

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

        public bool TryEnterFullScreen()
        {
            return AppView.TryEnterFullScreenMode();
        }

        public void ExitFullScreen()
        {
            AppView.ExitFullScreenMode();
        }

        public async Task<IPlatformFileWrapper> SelectFileAsync(IEnumerable<string> extensionsFilter)
        {
            var picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            foreach (var i in extensionsFilter)
            {
                picker.FileTypeFilter.Add(i);
            }

            var file = await picker.PickSingleFileAsync();
            return file == null ? null : new PlatformFileWrapper(file);
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

                //Alt+Enter: enter fullscreen
                case VirtualKey.Enter:
                    if (CoreWindow.GetKeyState(VirtualKey.Menu) == CoreVirtualKeyStates.Down)
                    {
                        TryEnterFullScreen();
                        args.Handled = true;
                    }
                    break;

                case VirtualKey.Escape:
                    ExitFullScreen();
                    args.Handled = true;
                    break;
            }
        }
    }
}
