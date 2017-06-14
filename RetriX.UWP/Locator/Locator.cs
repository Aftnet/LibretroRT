using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using LibretroRT.FrontendComponents.AudioGraphPlayer;
using LibretroRT.FrontendComponents.Common;
using LibretroRT.FrontendComponents.InputManager;
using Microsoft.Practices.ServiceLocation;
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
            ioc.Register<IMessenger, Messenger>();
            ioc.Register<IAudioPlayer, AudioGraphPlayer>();
            ioc.Register<IInputManager, InputManager>();
            ioc.Register<IPlatformService, PlatformService>();
            ioc.Register<IEmulationService, EmulationService>();
            ioc.Register<ISaveStateService, SaveStateService>();
            ioc.Register<ILocalizationService, LocalizationService>();
            ioc.Register<GameSystemSelectionVM>();
            ioc.Register<GamePlayerVM>();

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
        }
    }
}
