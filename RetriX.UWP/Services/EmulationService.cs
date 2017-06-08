using LibretroRT;
using LibretroRT.FrontendComponents.Common;
using RetriX.Shared.Services;
using RetriX.UWP.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RetriX.UWP.Services
{
    public class EmulationService : IEmulationService
    {
        private const char CoreExtensionDelimiter = '|';

        private static readonly IReadOnlyDictionary<GameSystemTypes, ICore> SystemCoreMapping = new Dictionary<GameSystemTypes, ICore>
        {
            { GameSystemTypes.NES, FCEUMMRT.FCEUMMCore.Instance },
            { GameSystemTypes.SNES, Snes9XRT.Snes9XCore.Instance },
            { GameSystemTypes.GB, GambatteRT.GambatteCore.Instance },
            { GameSystemTypes.GBA, VBAMRT.VBAMCore.Instance },
            { GameSystemTypes.MegaDrive, GPGXRT.GPGXCore.Instance },
        };

        public ICoreRunner CoreRunner { get; set; }
        public bool IsFullScreenMode => AppView.IsFullScreenMode;

        private ApplicationView AppView => ApplicationView.GetForCurrentView();
        private Frame RootFrame => Window.Current.Content as Frame;

        public async void SelectAndRunGame(GameSystemTypes systemType)
        {
            var core = SystemCoreMapping[systemType];
            var file = await PickCoreSupportedGameFile(core);
            if (file == null)
                return;

            RootFrame.Navigate(typeof(GamePlayerPage));
            var task = Task.Run(() => CoreRunner.LoadGame(core, file));
        }

        public bool TryEnterFullScreen()
        {
            return AppView.TryEnterFullScreenMode();
        }

        public void ExitFullScreen()
        {
            AppView.ExitFullScreenMode();
        }

        private Task<StorageFile> PickCoreSupportedGameFile(ICore core)
        {
            var picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            var extensions = core.SupportedExtensions.Split(CoreExtensionDelimiter).Select(d => $".{d}").ToArray();
            foreach (var i in extensions)
            {
                picker.FileTypeFilter.Add(i);
            }

            return picker.PickSingleFileAsync().AsTask();
        }
    }
}
