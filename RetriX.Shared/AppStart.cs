using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Plugin.FileSystem.Abstractions;
using RetriX.Shared.ViewModels;

namespace RetriX.Shared
{
    public class AppStart : IMvxAppStart
    {
        private IMvxNavigationService NavigationService { get; } = Mvx.Resolve<IMvxNavigationService>();

        public void Start(object hint = null)
        {
            var file = hint as IFileInfo;
            if (file != null)
            {
                NavigationService.Navigate<GameSystemSelectionViewModel, IFileInfo>(file);
                return;
            }

            NavigationService.Navigate<GameSystemSelectionViewModel>();
        }
    }
}
