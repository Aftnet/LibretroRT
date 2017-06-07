using LibretroRT.FrontendComponents.Win2DRenderer;
using Microsoft.Practices.ServiceLocation;
using RetriX.Shared.Services;
using RetriX.UWP.Services;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RetriX.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePlayerPageWin2D : Page
    {
        private IServiceLocator Locator => ServiceLocator.Current;

        private readonly Win2DRenderer Renderer;

        private readonly EmulationService EmulationService;

        public GamePlayerPageWin2D()
        {
            this.InitializeComponent();

            EmulationService = Locator.GetInstance<IEmulationService>() as EmulationService;
            Renderer = new Win2DRenderer(Win2dCanvas, EmulationService.AudioPlayer, EmulationService.InputManager);
            EmulationService.CoreRunner = Renderer;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Renderer.Dispose();
        }
    }
}
