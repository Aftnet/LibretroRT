using LibretroRT;
using LibretroRT.FrontendComponents.Common;
using PCLStorage;
using RetriX.Shared.FileProviders;
using RetriX.Shared.Services;
using RetriX.UWP.FileProviders;
using RetriX.UWP.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            { GameSystemTypes.SG1000, GPGXRT.GPGXCore.Instance },
            { GameSystemTypes.MasterSystem, GPGXRT.GPGXCore.Instance },
            { GameSystemTypes.GameGear, GPGXRT.GPGXCore.Instance },
            { GameSystemTypes.MegaDrive, GPGXRT.GPGXCore.Instance },
        };

        private readonly ILocalizationService LocalizationService;
        private readonly IPlatformService PlatformService;

        private readonly Frame RootFrame = Window.Current.Content as Frame;

        private IFileProvider StreamProvider;
        private ICoreRunner CoreRunner;

        public string GameID => CoreRunner?.GameID;

        public event GameStartedDelegate GameStarted;
        public event GameRuntimeExceptionOccurredDelegate GameRuntimeExceptionOccurred;

        public EmulationService(ILocalizationService localizationService, IPlatformService platformService)
        {
            LocalizationService = localizationService;
            PlatformService = platformService;

            RootFrame.Navigated += OnNavigated;
        }

        public IReadOnlyList<string> GetSupportedExtensions(GameSystemTypes systemType)
        {
            return SystemCoreMapping[systemType].SupportedExtensions;
        }

        public Task<bool> StartGameAsync(IFile file)
        {
            if (file == null)
            {
                throw new ArgumentException();
            }

            var fileExtension = System.IO.Path.GetExtension(file.Path);
            foreach (var i in SystemCoreMapping.Values)
            {
                if (i.SupportedExtensions.Contains(fileExtension))
                {
                    return StartGameAsync(i, file);
                }
            }

            throw new Exception("No compatible core found");
        }

        public Task<bool> StartGameAsync(GameSystemTypes systemType, IFile file)
        {
            if (file == null)
            {
                throw new ArgumentException();
            }

            var core = SystemCoreMapping[systemType];
            return StartGameAsync(core, file);
        }

        private async Task<bool> StartGameAsync(ICore core, IFile file)
        {
            if (CoreRunner == null)
            {
                RootFrame.Navigate(typeof(GamePlayerPage));
            }

            //Navigation should cause the player page to load, which in turn should initialize the core runner
            while (CoreRunner == null)
            {
                await Task.Delay(100);
            }

            return await StartGameAsync(CoreRunner, core, file);
        }

        private async Task<bool> StartGameAsync(ICoreRunner runner, ICore core, IFile file)
        {
            var mainGamePath = $"ROM\\{file.Name}";
            StreamProvider = new SingleFileProvider(mainGamePath, file);
            core.GetFileStream = (d, e) => StreamProvider.GetFileStreamAsync(d, e.ToIOAccess()).Result?.AsRandomAccessStream();

            var loadSuccessful = await runner.LoadGameAsync(core, mainGamePath);
            if (loadSuccessful)
            {
                GameStarted(this);
            }
            else
            {
                RootFrame.GoBack();
                return false;
            }

            return loadSuccessful;
        }

        public Task ResetGameAsync()
        {
            return CoreRunner?.ResetGameAsync().AsTask();
        }

        public async Task StopGameAsync()
        {
            await CoreRunner?.UnloadGameAsync();
            RootFrame.GoBack();
        }

        public Task PauseGameAsync()
        {
            return CoreRunner != null ? CoreRunner.PauseCoreExecutionAsync().AsTask() : Task.CompletedTask;
        }

        public Task ResumeGameAsync()
        {
            return CoreRunner != null ? CoreRunner.ResumeCoreExecutionAsync().AsTask() : Task.CompletedTask;
        }

        public async Task<byte[]> SaveGameStateAsync()
        {
            if (CoreRunner == null)
            {
                return null;
            }

            var output = new byte[CoreRunner.SerializationSize];
            var success = await CoreRunner.SaveGameStateAsync(output);
            return success ? output : null;
        }

        public Task<bool> LoadGameStateAsync(byte[] stateData)
        {
            if (CoreRunner == null)
            {
                return Task.FromResult(false);
            }

            return CoreRunner.LoadGameStateAsync(stateData).AsTask();
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            var runnerPage = e.Content as ICoreRunnerPage;
            CoreRunner = runnerPage?.CoreRunner;

            if (CoreRunner != null)
            {
                CoreRunner.CoreRunExceptionOccurred -= OnCoreExceptionOccurred;
                CoreRunner.CoreRunExceptionOccurred += OnCoreExceptionOccurred;
            }
        }

        private void OnCoreExceptionOccurred(ICore core, Exception e)
        {
            var task = PlatformService.RunOnUIThreadAsync(() =>
            {
                RootFrame.GoBack();
                GameRuntimeExceptionOccurred(this, e);
            });
        }
    }
}
