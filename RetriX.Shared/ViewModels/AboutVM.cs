using MvvmCross.Core.ViewModels;
using Plugin.VersionTracking.Abstractions;

namespace RetriX.Shared.ViewModels
{
    public class AboutVM : MvxViewModel
    {
        public string Version { get; }

        public AboutVM(IVersionTracking versionTracker)
        {
            Version = versionTracker.CurrentVersion;
        }
    }
}