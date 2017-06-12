using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;

namespace LibretroRT.FrontendComponents.Common
{
    public interface ICoreRunner
    {
        bool CoreIsExecuting { get; }

        void LoadGame(ICore core, IStorageFile gameFile);
        void UnloadGame();
        void ResetGame();

        void PauseCoreExecution();
        void ResumeCoreExecution();

        //bool LoadState(IStorageFile stateFile);
        //bool SaveState(IStorageFile stateFile);
    }
}
