using GalaSoft.MvvmLight;
using RetriX.Shared.Services;

namespace RetriX.Shared.ViewModels
{
    public class GameSystemListItemVM : ViewModelBase
    {
        private GameSystemTypes type;
        public GameSystemTypes Type
        {
            get { return type; }
            set { Set(ref type, value); }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { Set(ref name, value); }
        }

        private string manufacturer;
        public string Manufacturer
        {
            get { return manufacturer; }
            set { Set(ref manufacturer, value); }
        }

        private string symbol;
        public string Symbol
        {
            get { return symbol; }
            set { Set(ref symbol, value); }
        }

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
