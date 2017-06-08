using LibretroRT.FrontendComponents.Common;
using LibretroRT.FrontendComponents.MonoGameRenderer;
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

        private readonly MonoGameCoreRunner Runner;

        private readonly EmulationService EmulationService;

        public GamePlayerPage()
        {
            this.InitializeComponent();

            EmulationService = Locator.GetInstance<IEmulationService>() as EmulationService;
            Runner = new MonoGameCoreRunner(PlayerPanel, Locator.GetInstance<IAudioPlayer>(), Locator.GetInstance<IInputManager>());
            EmulationService.CoreRunner = Runner;

            Unloaded += OnUnloading;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Runner.UnloadGame();
        }

        private void OnUnloading(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            EmulationService.CoreRunner = null;
            Runner.Dispose();
        }
    }
}
