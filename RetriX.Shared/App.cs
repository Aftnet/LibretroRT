using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using RetriX.Shared.Services;

namespace RetriX.Shared
{
    public class App : MvxApplication
    {
        public App()
        {
            Mvx.RegisterSingleton(Acr.UserDialogs.UserDialogs.Instance);
            Mvx.RegisterSingleton(Plugin.FileSystem.CrossFileSystem.Current);
            Mvx.RegisterSingleton(Plugin.LocalNotifications.CrossLocalNotifications.Current);
            Mvx.RegisterSingleton(Plugin.VersionTracking.CrossVersionTracking.Current);
            Mvx.ConstructAndRegisterSingleton<IMvxNavigationService, MvxNavigationService>();
            Mvx.ConstructAndRegisterSingleton<ICryptographyService, CryptographyService>();
            Mvx.ConstructAndRegisterSingleton<ISaveStateService, SaveStateService>();
            Mvx.LazyConstructAndRegisterSingleton<IEmulationService, EmulationService>();
            RegisterCustomAppStart<AppStart>();
        }
    }
}
