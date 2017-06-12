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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

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

        private readonly IPlatformService PlatformService;
        private readonly Frame RootFrame = Window.Current.Content as Frame;

        private ICoreRunner CoreRunner;
        private Tuple<ICore, IStorageFile> GameRunRequest;

        public bool GamePaused
        {
            get { return CoreRunner != null ? !CoreRunner.CoreIsExecuting : true; }
            set
            {
                if (value == true)
                {
                    CoreRunner?.PauseCoreExecution();
                }
                else
                {
                    CoreRunner?.ResumeCoreExecution();
                }
            }
        }

        public EmulationService(IPlatformService platformService)
        {
            PlatformService = platformService;
            RootFrame.Navigated += OnNavigated;
        }

        public async void SelectAndRunGameForSystem(GameSystemTypes systemType)
        {
            var core = SystemCoreMapping[systemType];
            var extensions = GetSupportedExtensionsListForCore(core);
            var file = await PlatformService.SelectFileAsync(extensions);
            if (file != null)
            {
                RunGame(core, file);
            }
        }

        public void RunGame(IPlatformFileWrapper file)
        {
            var platformFile = file.File as IStorageFile;
            foreach (var i in SystemCoreMapping.Values)
            {
                var coreExtensions = GetSupportedExtensionsListForCore(i);
                if (coreExtensions.Contains(platformFile.FileType))
                {
                    GameRunRequest = new Tuple<ICore, IStorageFile>(i, platformFile);
                    ExecuteGameRunRequest();
                    return;
                }
            }
        }

        public void ResetGame()
        {
            CoreRunner?.ResetGame();
        }

        private void RunGame(ICore core, IPlatformFileWrapper file)
        {
            GameRunRequest = new Tuple<ICore, IStorageFile>(core, file.File as IStorageFile);
            ExecuteGameRunRequest();
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            var runnerPage = e.Content as ICoreRunnerPage;
            CoreRunner = runnerPage?.CoreRunner;
            PlatformService.HandleGameplayKeyShortcuts = runnerPage != null;

            ExecuteGameRunRequest();
        }

        private void ExecuteGameRunRequest()
        {
            if (GameRunRequest == null)
                return;

            if (CoreRunner != null)
            {
                //Need to null GameRunRequest before starting another thread
                var request = GameRunRequest;
                GameRunRequest = null;
                var task = Task.Run(() => CoreRunner.LoadGame(request.Item1, request.Item2));
            }
            else
            {
                RootFrame.Navigate(typeof(GamePlayerPage));
            }
        }

        private Task<StorageFile> PickCoreSupportedGameFile(ICore core)
        {
            var picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            var coreExtensions = GetSupportedExtensionsListForCore(core);
            foreach (var i in coreExtensions)
            {
                picker.FileTypeFilter.Add(i);
            }

            return picker.PickSingleFileAsync().AsTask();
        }

        private string[] GetSupportedExtensionsListForCore(ICore core)
        {
            return core.SupportedExtensions.Split(CoreExtensionDelimiter).Select(d => $".{d}").ToArray();
        }
    }
}
