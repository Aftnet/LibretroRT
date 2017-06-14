using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetriX.Shared.Services
{
    public enum GameSystemTypes { NES, SNES, GB, GBA, MegaDrive };

    public interface IEmulationService
    {
        string GameID { get; }

        IReadOnlyList<string> GetSupportedExtensions(GameSystemTypes systemType);

        Task<bool> StartGameAsync(IPlatformFileWrapper file);
        Task<bool> StartGameAsync(GameSystemTypes systemType, IPlatformFileWrapper file);
        Task ResetGameAsync();

        Task PauseGameAsync();
        Task ResumeGameAsync();

        Task<byte[]> SaveGameStateAsync();
        Task<bool> LoadGameStateAsync(byte[] stateData);
    }
}
