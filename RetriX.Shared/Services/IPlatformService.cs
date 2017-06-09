namespace RetriX.Shared.Services
{
    public interface IPlatformService
    {
        bool IsFullScreenMode { get; }
        bool HandleGameplayKeyShortcuts { get; set; }

        bool TryEnterFullScreen();
        void ExitFullScreen();
    }
}