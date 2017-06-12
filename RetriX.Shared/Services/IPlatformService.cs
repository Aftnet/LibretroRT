using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetriX.Shared.Services
{
    public interface IPlatformService
    {
        bool IsFullScreenMode { get; }
        bool HandleGameplayKeyShortcuts { get; set; }

        bool TryEnterFullScreen();
        void ExitFullScreen();

        Task<IPlatformFileWrapper> SelectFileAsync(IEnumerable<string> extensionsFilter);

        Task RunOnUIThreadAsync(Action action);
    }
}