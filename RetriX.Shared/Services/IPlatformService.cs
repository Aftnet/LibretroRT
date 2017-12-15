using System;
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
        bool FullScreenChangingPossible { get; }
        bool IsFullScreenMode { get; }
        bool ShouldDisplayTouchGamepad { get; }
        bool HandleGameplayKeyShortcuts { get; set; }

        bool ChangeFullScreenState(FullScreenChangeType changeType);
        void ChangeMousePointerVisibility(MousePointerVisibility visibility);
        void ForceUIElementFocus();

        void CopyToClipboard(string content);

        Task RunOnUIThreadAsync(Action action);

        event FullScreenChangeRequestedDelegate FullScreenChangeRequested;

        event PauseToggleRequestedDelegate PauseToggleRequested;

        event GameStateOperationRequestedDelegate GameStateOperationRequested;
    }
}