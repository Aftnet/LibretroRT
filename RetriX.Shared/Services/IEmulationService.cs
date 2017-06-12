namespace RetriX.Shared.Services
{
    public enum GameSystemTypes { NES, SNES, GB, GBA, MegaDrive };

    public delegate void GamePausedChangedDelegate();

    public interface IEmulationService
    {
        bool GamePaused { get; set; }

        void SelectAndRunGameForSystem(GameSystemTypes systemType);
        void RunGame(IPlatformFileWrapper file);
        void ResetGame();

        event GamePausedChangedDelegate GamePausedChanged;
    }
}
