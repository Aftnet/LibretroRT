using LibRetriX;
using System;
using System.IO;
using Windows.Foundation;

namespace LibretroRT.FrontendComponents.Common
{
    public delegate void CoreRunExceptionOccurredDelegate(ICore core, Exception e);

    public interface ICoreRunner
    {
        string GameID { get; }
        bool CoreIsExecuting { get; }
        ulong SerializationSize { get; }

        IAsyncOperation<bool> LoadGameAsync(ICore core, string mainGameFilePath);
        IAsyncAction UnloadGameAsync();
        IAsyncAction ResetGameAsync();

        IAsyncAction PauseCoreExecutionAsync();
        IAsyncAction ResumeCoreExecutionAsync();

        IAsyncOperation<bool> SaveGameStateAsync(Stream outputStream);
        IAsyncOperation<bool> LoadGameStateAsync(Stream inputStream);

        event CoreRunExceptionOccurredDelegate CoreRunExceptionOccurred;
    }
}
