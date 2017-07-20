using PCLStorage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetriX.Shared.Services
{
    public enum FullScreenChangeType { Enter, Exit, Toggle };

    public enum MousePointerVisibility { Visible, Hidden };

    public delegate void FullScreenChangeRequestedDelegate(IPlatformService sender, FullScreenChangeEventArgs args);

    public delegate void PauseToggleRequestedDelegate(IPlatformService sender);

    public delegate void GameStateOperationRequestedDelegate(IPlatformService sender, GameStateOperationEventArgs args);

    public interface IPlatformService
    {
        bool IsFullScreenMode { get; }
        bool HandleGameplayKeyShortcuts { get; set; }

        bool ChangeFullScreenState(FullScreenChangeType changeType);
        void ChangeMousePointerVisibility(MousePointerVisibility visibility);

        Task<IFile> SelectFileAsync(IEnumerable<string> extensionsFilter);
        Task<IFolder> SelectFolderAsync();
        void CopyToClipboard(string content);

        Task RunOnUIThreadAsync(Action action);

        event FullScreenChangeRequestedDelegate FullScreenChangeRequested;

        event PauseToggleRequestedDelegate PauseToggleRequested;

        event GameStateOperationRequestedDelegate GameStateOperationRequested;
    }
}