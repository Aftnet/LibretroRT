using Acr.UserDialogs;
using GalaSoft.MvvmLight.Ioc;
using LibretroRT.FrontendComponents.AudioGraphPlayer;
using LibretroRT.FrontendComponents.Common;
using LibretroRT.FrontendComponents.InputManager;
using Microsoft.Practices.ServiceLocation;
using Plugin.FileSystem;
using Plugin.LocalNotifications;
using Plugin.VersionTracking;
using RetriX.Shared.Services;
using RetriX.Shared.ViewModels;
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
            ioc.Register(() => UserDialogs.Instance);
            ioc.Register(() => CrossFileSystem.Current);
            ioc.Register(() => CrossLocalNotifications.Current);
            ioc.Register(() => CrossVersionTracking.Current);
            ioc.Register<IAudioPlayer, AudioGraphPlayer>();
            ioc.Register<IInputManager, InputManager>();
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
    }
}
