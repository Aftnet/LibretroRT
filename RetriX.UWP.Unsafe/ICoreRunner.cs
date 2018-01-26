using LibRetriX;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RetriX.UWP
{
    public delegate void CoreRunExceptionOccurredDelegate(ICore core, Exception e);

    public interface ICoreRunner
    {
        string GameID { get; }
        bool CoreIsExecuting { get; }
        ulong SerializationSize { get; }

        Task<bool> LoadGameAsync(ICore core, string mainGameFilePath);
        Task UnloadGameAsync();
        Task ResetGameAsync();

        Task PauseCoreExecutionAsync();
        Task ResumeCoreExecutionAsync();

        Task<bool> SaveGameStateAsync(Stream outputStream);
        Task<bool> LoadGameStateAsync(Stream inputStream);

        event CoreRunExceptionOccurredDelegate CoreRunExceptionOccurred;
    }
}
