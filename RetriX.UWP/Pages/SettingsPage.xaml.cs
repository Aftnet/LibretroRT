using Microsoft.Practices.ServiceLocation;
using RetriX.Shared.ViewModels;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace RetriX.UWP.Pages
{
    public sealed partial class SettingsPage : UserControl, ITypedViewModel<SettingsVM>
    {
        public SettingsVM VM => ServiceLocator.Current.GetInstance<SettingsVM>();

        public SettingsPage()
        {
            this.InitializeComponent();
        }
    }
}
