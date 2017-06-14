using RetriX.Shared.Services;

namespace RetriX.Shared.ViewModels
{
    public class GameSystemListItemVM
    {
        public GameSystemTypes Type { get; private set; }

        public string Name { get; private set; }

        public string Manufacturer { get; private set; }

        public string Symbol { get; private set; }

        public GameSystemListItemVM()
        {

        }

        public GameSystemListItemVM(ILocalizationService localizer, GameSystemTypes type, string nameResKey, string manufacturerResKey, string symbol)
        {
            Type = type;
            Name = localizer.GetLocalizedString(nameResKey);
            Manufacturer = localizer.GetLocalizedString(manufacturerResKey);
            Symbol = symbol;
        }
    }
}
