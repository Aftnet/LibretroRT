using GalaSoft.MvvmLight;
using Plugin.VersionTracking.Abstractions;

namespace RetriX.Shared.ViewModels
{
    public class AboutVM : ViewModelBase
    {
        private readonly string version;
        public string Version { get { return version; } }

        public AboutVM(IVersionTracking versionTracker)
        {
            version = versionTracker.CurrentVersion;
        }
    }
}