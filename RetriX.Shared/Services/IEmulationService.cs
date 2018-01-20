using Plugin.FileSystem.Abstractions;
using RetriX.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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

    public delegate void GameStoppedDelegate(IEmulationService sender);
    public delegate void GameStartedDelegate(IEmulationService sender);
    public delegate void GameRuntimeExceptionOccurredDelegate(IEmulationService sender, Exception exception);

    public interface IEmulationService
    {
        IReadOnlyList<GameSystemVM> Systems { get; }
        IReadOnlyList<string> ArchiveExtensions { get; }
        IReadOnlyList<FileImporterVM> FileDependencyImporters { get; }

        string GameID { get; }

        IEnumerable<GameSystemVM> FilterSystemsForFile(IFileInfo file);
        Task<bool> StartGameAsync(GameSystemVM system, IFileInfo file, IDirectoryInfo rootFolder = null);

        Task ResetGameAsync();
        Task StopGameAsync();

        Task PauseGameAsync();
        Task ResumeGameAsync();

        Task<bool> SaveGameStateAsync(Stream outputData);
        Task<bool> LoadGameStateAsync(Stream inputStream);

        void InjectInputPlayer1(InjectedInputTypes inputType);

        event GameStartedDelegate GameStarted;
        event GameStoppedDelegate GameStopped;
        event GameRuntimeExceptionOccurredDelegate GameRuntimeExceptionOccurred;
    }
}
