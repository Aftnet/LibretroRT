using PCLStorage;
using RetriX.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetriX.Shared.Services
{
    public enum InjectedInputTypes
    {
        DeviceIdJoypadB = 0,
        DeviceIdJoypadY = 1,
        DeviceIdJoypadSelect = 2,
        DeviceIdJoypadStart = 3,
        DeviceIdJoypadUp = 4,
        DeviceIdJoypadDown = 5,
        DeviceIdJoypadLeft = 6,
        DeviceIdJoypadRight = 7,
        DeviceIdJoypadA = 8,
        DeviceIdJoypadX = 9,
    };

    public delegate void CoresInitializedDelegate(IEmulationService sender);
    public delegate void GameStartedDelegate(IEmulationService sender);
    public delegate void GameRuntimeExceptionOccurredDelegate(IEmulationService sender, Exception exception);

    public interface IEmulationService
    {
        IReadOnlyList<string> ArchiveExtensions { get; }

        IReadOnlyList<FileImporterVM> FileDependencyImporters { get; }
        string GameID { get; }

        Task ResetGameAsync();
        Task StopGameAsync();

        Task PauseGameAsync();
        Task ResumeGameAsync();

        Task<byte[]> SaveGameStateAsync();
        Task<bool> LoadGameStateAsync(byte[] stateData);

        void InjectInputPlayer1(InjectedInputTypes inputType);

        event CoresInitializedDelegate CoresInitialized;
        event GameStartedDelegate GameStarted;
        event GameRuntimeExceptionOccurredDelegate GameRuntimeExceptionOccurred;
    }

    public interface IEmulationService<T> : IEmulationService where T : GameSystemVMBase
    {
        IReadOnlyList<T> Systems { get; }

        Task<T> SuggestSystemForFileAsync(IFile file);
        bool CheckRootFolderRequired(T system, IFile file);
        Task<bool> CheckDependenciesMetAsync(T system);
        Task<bool> StartGameAsync(T system, IFile file, IFolder rootFolder = null);
    }
}
