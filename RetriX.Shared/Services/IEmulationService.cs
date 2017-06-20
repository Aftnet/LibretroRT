using PCLStorage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetriX.Shared.Services
{
    public enum GameSystemTypes { NES, SNES, GB, GBA, SG1000, MasterSystem, GameGear, MegaDrive };

    public interface IEmulationService
    {
        string GameID { get; }

        IReadOnlyList<string> GetSupportedExtensions(GameSystemTypes systemType);

        Task<bool> StartGameAsync(IFile file);
        Task<bool> StartGameAsync(GameSystemTypes systemType, IFile file);
        Task ResetGameAsync();
        Task StopGameAsync();

        Task PauseGameAsync();
        Task ResumeGameAsync();

        Task<byte[]> SaveGameStateAsync();
        Task<bool> LoadGameStateAsync(byte[] stateData);
    }
}
