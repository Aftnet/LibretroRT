using LibretroRT.FrontendComponents.Common;
using LibretroRT.FrontendComponents.Win2DCoreRunner;
using Microsoft.Practices.ServiceLocation;
using RetriX.Shared.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RetriX.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePlayerPage : Page, ITypedViewModel<GamePlayerVM>, ICoreRunnerPage
    {
        public GamePlayerVM VM => ServiceLocator.Current.GetInstance<GamePlayerVM>();
        public ICoreRunner CoreRunner => Runner;

        private Win2DCoreRunner Runner;

        public GamePlayerPage()
        {
            InitializeComponent();

            var locator = ServiceLocator.Current;
            var audioPlayer = locator.GetInstance<IAudioPlayer>();
            var inputManager = locator.GetInstance<IInputManager>();
            Runner = new Win2DCoreRunner(PlayerPanel, audioPlayer, inputManager);

            Unloaded += OnUnloading;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            VM.Activated();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            VM.Deactivated();
            base.OnNavigatingFrom(e);
        }

        private void OnUnloading(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Runner.Dispose();
            Runner = null;

            PlayerPanel.RemoveFromVisualTree();
            PlayerPanel = null;
        }
    }
}
