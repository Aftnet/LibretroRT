using Acr.UserDialogs;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Plugin.FileSystem;
using Plugin.LocalNotifications;
using Plugin.VersionTracking;
using RetriX.Shared.Services;
using RetriX.Shared.ViewModels;
using RetriX.UWP.Pages;
using RetriX.UWP.Services;

namespace RetriX.UWP.Locator
{
    public class Locator
    {
        public static void Initialize()
        {
            if (ServiceLocator.IsLocationProviderSet)
                return;

            var ioc = SimpleIoc.Default;
            ioc.Register(GetNavigationService);
            ioc.Register(() => UserDialogs.Instance);
            ioc.Register(() => CrossFileSystem.Current);
            ioc.Register(() => CrossLocalNotifications.Current);
            ioc.Register(() => CrossVersionTracking.Current);
            ioc.Register<IVideoService, VideoService>();
            ioc.Register<IAudioService, AudioService>();
            ioc.Register<IInputService, InputService>();
            ioc.Register<IPlatformService, PlatformService>();
            ioc.Register<ICryptographyService, CryptographyService>();
            ioc.Register<IEmulationService, EmulationService>();
            ioc.Register<ISaveStateService, SaveStateService>();
            ioc.Register<ILocalizationService, LocalizationService>();
            ioc.Register<GameSystemSelectionVM>();
            ioc.Register<AboutVM>();
            ioc.Register<GamePlayerVM>();
            ioc.Register<SettingsVM>();

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
        }

        private static INavigationService GetNavigationService()
        {
            var output = new NavigationService();
            output.Configure(nameof(GamePlayerVM), typeof(GamePlayerPage));
            return output;
        }
    }
}
