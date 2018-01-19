using LibRetriX;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;

namespace LibretroRT.FrontendComponents.Common
{
    public delegate void CoreRunExceptionOccurredDelegate(ICore core, Exception e);

    public interface ICoreRunner
    {
        string GameID { get; }
        bool CoreIsExecuting { get; }
        uint SerializationSize { get; }

        IAsyncOperation<bool> LoadGameAsync(ICore core, string mainGameFilePath);
        IAsyncAction UnloadGameAsync();
        IAsyncAction ResetGameAsync();

        IAsyncAction PauseCoreExecutionAsync();
        IAsyncAction ResumeCoreExecutionAsync();

        IAsyncOperation<bool> SaveGameStateAsync([WriteOnlyArray] byte[] stateData);
        IAsyncOperation<bool> LoadGameStateAsync([ReadOnlyArray] byte[] stateData);

        event CoreRunExceptionOccurredDelegate CoreRunExceptionOccurred;
    }
}
