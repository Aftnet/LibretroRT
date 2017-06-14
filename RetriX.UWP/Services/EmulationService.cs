using LibretroRT;
using LibretroRT.FrontendComponents.Common;
using RetriX.Shared.Services;
using RetriX.UWP.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
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
                GamePausedChanged();
            }
        }

        public event GamePausedChangedDelegate GamePausedChanged;

        public EmulationService()
        {
            RootFrame.Navigated += OnNavigated;
        }

        public IReadOnlyList<string> GetSupportedExtensions(GameSystemTypes systemType)
        {
            var core = SystemCoreMapping[systemType];
            return GetSupportedExtensionsListForCore(core);
        }

        public Task RunGameAsync(IPlatformFileWrapper file)
        {
            if (file == null)
            {
                throw new ArgumentException();
            }

            var platformFile = file.File as IStorageFile;
            foreach (var i in SystemCoreMapping.Values)
            {
                var coreExtensions = GetSupportedExtensionsListForCore(i);
                if (coreExtensions.Contains(platformFile.FileType))
                {
                    return RunGameAsync(i, file);
                }
            }

            throw new Exception("No compatible core found");
        }

        public Task RunGameAsync(GameSystemTypes systemType, IPlatformFileWrapper file)
        {
            if (file == null)
            {
                throw new ArgumentException();
            }

            var core = SystemCoreMapping[systemType];
            return RunGameAsync(core, file);
        }

        private Task RunGameAsync(ICore core, IPlatformFileWrapper file)
        {
            GameRunRequest = new Tuple<ICore, IStorageFile>(core, file.File as IStorageFile);
            return ExecuteGameRunRequestAsync();
        }

        public Task ResetGameAsync()
        {
            return Task.Run(() => CoreRunner?.ResetGame());
        }

        public async Task<byte[]> SaveGameStateAsync()
        {
            if (CoreRunner == null)
            {
                return null;
            }

            var output = new byte[CoreRunner.SerializationSize];
            var success = await Task.Run(() => CoreRunner.SaveGameState(output));
            return success ? output : null;
        }

        public Task<bool> LoadGameStateAsync(byte[] stateData)
        {
            if (CoreRunner == null)
            {
                return Task.FromResult(false);
            }

            return Task.Run(() => CoreRunner.LoadGameState(stateData));
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            var runnerPage = e.Content as ICoreRunnerPage;
            CoreRunner = runnerPage?.CoreRunner;
            var task = ExecuteGameRunRequestAsync();
        }

        private async Task ExecuteGameRunRequestAsync()
        {
            if (GameRunRequest == null)
                return;

            if (CoreRunner != null)
            {
                //Need to null GameRunRequest before starting another thread
                var request = GameRunRequest;
                GameRunRequest = null;
                await Task.Run(() => CoreRunner.LoadGame(request.Item1, request.Item2));
                GamePausedChanged();
            }
            else
            {
                RootFrame.Navigate(typeof(GamePlayerPage));
            }
        }

        private string[] GetSupportedExtensionsListForCore(ICore core)
        {
            return core.SupportedExtensions.Split(CoreExtensionDelimiter).Select(d => $".{d}").ToArray();
        }
    }
}
