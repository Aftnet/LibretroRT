using Microsoft.Practices.ServiceLocation;
using RetriX.Shared.ViewModels;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RetriX.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutPage : Page, ITypedViewModel<AboutVM>
    {
        public AboutVM VM => ServiceLocator.Current.GetInstance<AboutVM>();

        public AboutPage()
        {
            this.InitializeComponent();
        }
    }
}
