using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;

namespace LibretroRT.FrontendComponents.Common
{
    public interface ICoreRunner
    {
        string GameID { get; }
        bool CoreIsExecuting { get; }
        uint SerializationSize { get; }

        void LoadGame(ICore core, IStorageFile gameFile);
        void UnloadGame();
        void ResetGame();

        void PauseCoreExecution();
        void ResumeCoreExecution();

        bool SaveGameState([WriteOnlyArray] byte[] stateData);
        bool LoadGameState([ReadOnlyArray] byte[] stateData);
    }
}
