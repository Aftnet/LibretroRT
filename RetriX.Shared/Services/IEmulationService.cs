using PCLStorage;
using RetriX.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetriX.Shared.Services
{
    public delegate Task<IFolder> RequestGameFolderAsyncDelegate(IEmulationService sender);

    public delegate void GameStartedDelegate(IEmulationService sender);
    public delegate void GameRuntimeExceptionOccurredDelegate(IEmulationService sender, Exception exception);

    public interface IEmulationService
    {
        IReadOnlyList<FileImporterVM> FileDependencyImporters { get; }
        string GameID { get; }

        Task<bool> StartGameAsync(IFile file);
        Task ResetGameAsync();
        Task StopGameAsync();

        Task PauseGameAsync();
        Task ResumeGameAsync();

        Task<byte[]> SaveGameStateAsync();
        Task<bool> LoadGameStateAsync(byte[] stateData);

        RequestGameFolderAsyncDelegate RequestGameFolderAsync { get; set; }

        event GameStartedDelegate GameStarted;
        event GameRuntimeExceptionOccurredDelegate GameRuntimeExceptionOccurred;
    }

    public interface IEmulationService<T> : IEmulationService where T : GameSystemVMBase
    {
        IReadOnlyList<T> Systems { get; }
        Task<bool> StartGameAsync(T system, IFile file);
    }
}
