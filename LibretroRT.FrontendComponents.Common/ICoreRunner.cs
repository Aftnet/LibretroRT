using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;

namespace LibretroRT.FrontendComponents.Common
{
    public interface ICoreRunner
    {
        void LoadGame(ICore core, IStorageFile gameFile);
        void UnloadGame();

        void PauseGame();
        void ResumeGame();
        void ResetGame();

        bool LoadState(IStorageFile stateFile);
        bool SaveState(IStorageFile stateFile);
    }
}
