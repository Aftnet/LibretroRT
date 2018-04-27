using MvvmCross.Uwp.Views;
using RetriX.Shared.ViewModels;

namespace RetriX.UWP.Pages
{
    public sealed partial class SystemSelectionView : MvxWindowsPage
    {
        public GameSystemSelectionViewModel VM => ViewModel as GameSystemSelectionViewModel;

        public SystemSelectionView()
        {
            this.InitializeComponent();
        }
    }
}
