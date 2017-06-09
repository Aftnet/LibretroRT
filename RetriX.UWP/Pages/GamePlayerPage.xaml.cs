﻿using LibretroRT.FrontendComponents.Common;
using LibretroRT.FrontendComponents.Win2DRenderer;
using Microsoft.Practices.ServiceLocation;
using RetriX.Shared.Services;
using RetriX.Shared.ViewModels;
using RetriX.UWP.Services;
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

        private Win2DRenderer Runner;

        private readonly IPlatformService PlatformService;
        private readonly EmulationService EmulationService;

        public GamePlayerPage()
        {
            this.InitializeComponent();

            PlatformService = Locator.GetInstance<IPlatformService>();
            EmulationService = Locator.GetInstance<IEmulationService>() as EmulationService;
            var audioPlayer = Locator.GetInstance<IAudioPlayer>();
            var inputManager = Locator.GetInstance<IInputManager>();
            Runner = new Win2DRenderer(PlayerPanel, audioPlayer, inputManager);
            EmulationService.CoreRunner = Runner;

            Unloaded += OnUnloading;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PlatformService.HandleGameplayKeyShortcuts = true;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            PlatformService.HandleGameplayKeyShortcuts = false;
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
