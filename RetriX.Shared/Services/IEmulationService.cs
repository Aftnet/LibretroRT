namespace RetriX.Shared.Services
{
    public enum GameSystemTypes { NES, SNES, GB, GBA, MegaDrive };

    public interface IEmulationService
    {
        bool IsFullScreenMode { get; }

        void SelectAndRunGame(GameSystemTypes systemType);

        bool TryEnterFullScreen();
        void ExitFullScreen();
    }
}
