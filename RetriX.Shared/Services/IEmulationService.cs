namespace RetriX.Shared.Services
{
    public enum GameSystemTypes { NES, SNES, GB, GBA, MegaDrive };

    public interface IEmulationService
    {
        void SelectAndRunGame(GameSystemTypes systemType);
    }
}
