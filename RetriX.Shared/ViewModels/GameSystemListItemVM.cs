using RetriX.Shared.Services;

namespace RetriX.Shared.ViewModels
{
    public class GameSystemListItemVM
    {
        public GameSystemTypes Type { get; private set; }
        public string Name { get; private set; }
        public string Symbol { get; private set; }

        public GameSystemListItemVM(GameSystemTypes type, string name, string symbol)
        {
            Type = type;
            Name = name;
            Symbol = symbol;
        }
    }
}
