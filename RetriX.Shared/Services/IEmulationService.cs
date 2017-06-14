using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetriX.Shared.Services
{
    public enum GameSystemTypes { NES, SNES, GB, GBA, MegaDrive };

    public delegate void GamePausedChangedDelegate();

    public interface IEmulationService
    {
        string GameID { get; }
        bool GamePaused { get; set; }

        IReadOnlyList<string> GetSupportedExtensions(GameSystemTypes systemType);

        Task RunGameAsync(IPlatformFileWrapper file);
        Task RunGameAsync(GameSystemTypes systemType, IPlatformFileWrapper file);
        Task ResetGameAsync();

        Task<byte[]> SaveGameStateAsync();
        Task<bool> LoadGameStateAsync(byte[] stateData);

        event GamePausedChangedDelegate GamePausedChanged;
    }
}
