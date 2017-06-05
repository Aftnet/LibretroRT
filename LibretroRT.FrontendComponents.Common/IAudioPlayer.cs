namespace LibretroRT.FrontendComponents.Common
{
    public interface IAudioPlayer
    {
        ICore Core { get; set; }

        bool ShouldDelayNextFrame { get; }

        void Stop();
    }
}