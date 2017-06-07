using GalaSoft.MvvmLight.Ioc;
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
            ioc.Register<IEmulationService, EmulationService>();
            ioc.Register<GameSystemSelectionVM>();

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
        }
    }
}
