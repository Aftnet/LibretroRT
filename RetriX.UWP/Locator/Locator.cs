using Acr.UserDialogs;
using GalaSoft.MvvmLight.Ioc;
using LibretroRT.FrontendComponents.AudioGraphPlayer;
using LibretroRT.FrontendComponents.Common;
using LibretroRT.FrontendComponents.InputManager;
using Microsoft.Practices.ServiceLocation;
using Plugin.LocalNotifications;
using Plugin.VersionTracking;
using RetriX.Shared.Services;
using RetriX.Shared.ViewModels;
using RetriX.UWP.Services;
using RetriX.UWP.ViewModels;
using System;

namespace RetriX.UWP.Locator
{
    public class Locator
    {
        private static Lazy<EmulationService> EmulationServiceLazy;

        static Locator()
        {
            EmulationServiceLazy = new Lazy<EmulationService>(() =>
            {
                var ioc = SimpleIoc.Default;
                return new EmulationService(ioc.GetInstance<ILocalizationService>(), ioc.GetInstance<IPlatformService>());
            });
        }

        public static void Initialize()
        {
            if (ServiceLocator.IsLocationProviderSet)
                return;

            var ioc = SimpleIoc.Default;
            ioc.Register(() => UserDialogs.Instance);
            ioc.Register(() => CrossLocalNotifications.Current);
            ioc.Register(() => CrossVersionTracking.Current);
            ioc.Register<IAudioPlayer, AudioGraphPlayer>();
            ioc.Register<IInputManager, InputManager>();
            ioc.Register<IPlatformService, PlatformService>();
            ioc.Register<IEmulationService<GameSystemVM>>(() => EmulationServiceLazy.Value);
            ioc.Register<IEmulationService>(() => EmulationServiceLazy.Value);
            ioc.Register<ISaveStateService, SaveStateService>();
            ioc.Register<ILocalizationService, LocalizationService>();
            ioc.Register<GameSystemSelectionVM<GameSystemVM>>();
            ioc.Register<AboutVM>();
            ioc.Register<GamePlayerVM>();

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
        }
    }
}
