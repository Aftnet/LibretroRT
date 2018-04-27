using MvvmCross.Uwp.Views;
using RetriX.Shared.ViewModels;

namespace RetriX.UWP.Pages
{
    public sealed partial class AboutPage : MvxWindowsPage
    {
        public AboutViewModel VM => ViewModel as AboutViewModel;

        public AboutPage()
        {
            this.InitializeComponent();
        }
    }
}
