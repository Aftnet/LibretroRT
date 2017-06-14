using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetriX.Shared.Services
{
    public delegate void GameStateOperationRequestedDelegate(IPlatformService sender, GameStateOperationEventArgs args);

    public interface IPlatformService
    {
        bool IsFullScreenMode { get; }
        bool HandleGameplayKeyShortcuts { get; set; }

        bool TryEnterFullScreen();
        void ExitFullScreen();
        void ToggleFullScreen();

        Task<IPlatformFileWrapper> SelectFileAsync(IEnumerable<string> extensionsFilter);

        Task RunOnUIThreadAsync(Action action);

        event GameStateOperationRequestedDelegate GameStateOperationRequested;
    }
}