using RetriX.Shared.Services;
using System.Collections.Generic;

namespace RetriX.Shared.ViewModels
{
    public class GameSystemListItemVM
    {
        private static readonly IReadOnlyList<GameSystemListItemVM> systems = new GameSystemListItemVM[]
        {
            new GameSystemListItemVM(GameSystemTypes.NES, "NES", "\ue928;"),
            new GameSystemListItemVM(GameSystemTypes.SNES, "SNES", "\ue90f"),
            new GameSystemListItemVM(GameSystemTypes.GB, "GameBoy", "\ue90d"),
            new GameSystemListItemVM(GameSystemTypes.GBA, "GameBoy Advance", "\ue90c"),
            new GameSystemListItemVM(GameSystemTypes.MegaDrive, "Mega Drive", "\ue91f"),
        };

        public static IReadOnlyList<GameSystemListItemVM> Systems => systems;

        public GameSystemTypes Type { get; private set; }
        public string Name { get; private set; }
        public string Symbol { get; private set; }

        private GameSystemListItemVM(GameSystemTypes type, string name, string symbol)
        {
            Type = type;
            Name = name;
            Symbol = symbol;
        }
    }
}
