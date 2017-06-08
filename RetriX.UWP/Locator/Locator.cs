using GalaSoft.MvvmLight.Ioc;
using LibretroRT.FrontendComponents.Common;
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
            ioc.Register<IAudioPlayer, IAudioPlayer>();
            ioc.Register<IInputManager, IInputManager>();
            ioc.Register<IEmulationService, EmulationService>();
            ioc.Register<ILocalizationService, LocalizationService>();
            ioc.Register<GameSystemSelectionVM>();

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
        }
    }
}
