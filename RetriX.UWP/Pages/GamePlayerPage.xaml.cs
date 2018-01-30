using Microsoft.Practices.ServiceLocation;
using RetriX.Shared.Services;
using RetriX.Shared.ViewModels;
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
        public GamePlayerVM VM => ServiceLocator.Current.GetInstance<GamePlayerVM>();

        private IEmulationService EmulationService => ServiceLocator.Current.GetInstance<IEmulationService>();
        private Win2DRenderer Renderer => ServiceLocator.Current.GetInstance<IVideoService>() as Win2DRenderer;

        public GamePlayerPage()
        {
            InitializeComponent();
            Unloaded += OnUnloading;

            Renderer.RenderPanel = PlayerPanel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            VM.Activated();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            VM.Deactivated();
            EmulationService.StopGameAsync();
            base.OnNavigatingFrom(e);
        }

        private void OnUnloading(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            PlayerPanel.RemoveFromVisualTree();
            PlayerPanel = null;
        }
    }
}
