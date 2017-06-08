using LibretroRT.FrontendComponents.Common;
using LibretroRT.FrontendComponents.Win2DRenderer;
using Microsoft.Practices.ServiceLocation;
using RetriX.Shared.Services;
using RetriX.Shared.ViewModels;
using RetriX.UWP.Services;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RetriX.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePlayerPage : Page, ITypedViewModel<GamePlayerVM>
    {
        public GamePlayerVM VM => Locator.GetInstance<GamePlayerVM>();

        private IServiceLocator Locator => ServiceLocator.Current;
        private CoreWindow CoreWindow => CoreWindow.GetForCurrentThread();

        private Win2DRenderer Runner;

        private readonly EmulationService EmulationService;

        public GamePlayerPage()
        {
            this.InitializeComponent();

            EmulationService = Locator.GetInstance<IEmulationService>() as EmulationService;
            var audioPlayer = Locator.GetInstance<IAudioPlayer>();
            var inputManager = Locator.GetInstance<IInputManager>();
            Runner = new Win2DRenderer(PlayerPanel, audioPlayer, inputManager);
            EmulationService.CoreRunner = Runner;

            Unloaded += OnUnloading;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var window = CoreWindow.GetForCurrentThread();
            window.KeyDown -= OnKeyDown;
            window.KeyDown += OnKeyDown;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            CoreWindow.KeyDown -= OnKeyDown;
        }

        private void OnKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            switch(args.VirtualKey)
            {
                //By default the gamepad's B button is treated as a hardware back button.
                //Handling the KeyDown event disables this.
                //We want this to happen only in the game page and not in the rest of the UI
                case VirtualKey.GamepadB:
                    args.Handled = true;
                    break;

                //Alt+Enter: enter fullscreen
                case VirtualKey.Enter:
                    if(CoreWindow.GetKeyState(VirtualKey.Menu) == CoreVirtualKeyStates.Down)
                    {
                        EmulationService.TryEnterFullScreen();
                        args.Handled = true;
                    }
                    break;

                case VirtualKey.Escape:
                    EmulationService.ExitFullScreen();
                    args.Handled = true;
                    break;
            }
        }

        private void OnUnloading(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            EmulationService.CoreRunner = null;
            Runner.Dispose();
            Runner = null;

            PlayerPanel.RemoveFromVisualTree();
            PlayerPanel = null;
        }
    }
}
