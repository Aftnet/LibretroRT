using RetriX.Shared.Services;

namespace RetriX.UWP.ViewModels
{
    public class GameSystemListItemVM : Shared.ViewModels.GameSystemListItemVM
    {
        public GameSystemListItemVM(ILocalizationService localizer, GameSystemTypes type, string nameResKey, string manufacturerResKey, string symbol)
            :base(localizer, type, nameResKey, manufacturerResKey, symbol)
        {
        }
    }
}
