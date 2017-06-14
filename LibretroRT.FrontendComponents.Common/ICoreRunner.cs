using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Storage;

namespace LibretroRT.FrontendComponents.Common
{
    public interface ICoreRunner
    {
        string GameID { get; }
        bool CoreIsExecuting { get; }
        uint SerializationSize { get; }

        IAsyncOperation<bool> LoadGameAsync(ICore core, IStorageFile gameFile);
        IAsyncAction UnloadGameAsync();
        IAsyncAction ResetGameAsync();

        IAsyncAction PauseCoreExecutionAsync();
        IAsyncAction ResumeCoreExecutionAsync();

        IAsyncOperation<bool> SaveGameStateAsync([WriteOnlyArray] byte[] stateData);
        IAsyncOperation<bool> LoadGameStateAsync([ReadOnlyArray] byte[] stateData);
    }
}
