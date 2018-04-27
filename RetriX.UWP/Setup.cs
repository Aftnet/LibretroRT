using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.Logging;
using MvvmCross.Uwp.Platform;
using RetriX.Shared.Services;
using RetriX.UWP.Services;
using Windows.UI.Xaml.Controls;

namespace RetriX.UWP
{
    public class Setup : MvxWindowsSetup
    {
        public Setup(Frame rootFrame) : base(rootFrame)
        {
        }

        protected override IMvxApplication CreateApp()
        {
            return new Shared.App();
        }

        protected override void InitializeFirstChance()
        {
            Mvx.ConstructAndRegisterSingleton<ILocalizationService, LocalizationService>();
            Mvx.ConstructAndRegisterSingleton<IPlatformService, PlatformService>();
            Mvx.ConstructAndRegisterSingleton<IInputService, InputService>();
            Mvx.ConstructAndRegisterSingleton<IAudioService, AudioService>();
            Mvx.ConstructAndRegisterSingleton<IVideoService, VideoService>();
        }

        protected override MvxLogProviderType GetDefaultLogProviderType()
        {
            return MvxLogProviderType.None;
        }
    }

}
