using RetriX.Shared.Services;
using Windows.ApplicationModel.Resources;

namespace RetriX.UWP.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly ResourceLoader ResourceLoader = ResourceLoader.GetForViewIndependentUse();
        public string GetLocalizedString(string key)
        {
            return ResourceLoader.GetString(key);
        }
    }
}
