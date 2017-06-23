using Acr.UserDialogs;
using LibretroRT;
using LibretroRT.FrontendComponents.Common;
using PCLStorage;
using RetriX.Shared.Services;
using RetriX.Shared.StreamProviders;
using RetriX.Shared.ViewModels;
using RetriX.UWP.Pages;
using RetriX.UWP.ViewModels;
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
    public class EmulationService : IEmulationService<GameSystemVM>
    {
        private const char CoreExtensionDelimiter = '|';

        private readonly ILocalizationService LocalizationService;
        private readonly IPlatformService PlatformService;

        private readonly Frame RootFrame = Window.Current.Content as Frame;

        private IStreamProvider StreamProvider;
        private ICoreRunner CoreRunner;

        private readonly ICore[] AvailableCores = { FCEUMMRT.FCEUMMCore.Instance, Snes9XRT.Snes9XCore.Instance, GambatteRT.GambatteCore.Instance, VBAMRT.VBAMCore.Instance, GPGXRT.GPGXCore.Instance };

        private readonly GameSystemVM[] systems;
        public IReadOnlyList<GameSystemVM> Systems => systems;

        private readonly Lazy<FileImporterVM[]> fileDependencyImporters;
        public IReadOnlyList<FileImporterVM> FileDependencyImporters => fileDependencyImporters.Value;

        public string GameID => CoreRunner?.GameID;

        public event GameStartedDelegate GameStarted;
        public event GameRuntimeExceptionOccurredDelegate GameRuntimeExceptionOccurred;

        public EmulationService(IUserDialogs dialogsService, ILocalizationService localizationService, IPlatformService platformService, ICryptographyService cryptographyService)
        {
            LocalizationService = localizationService;
            PlatformService = platformService;

            RootFrame.Navigated += OnNavigated;

            var CDImageExtensions = new HashSet<string> { ".bin", ".cue", ".iso" };
            systems = new GameSystemVM[]
            {
                new GameSystemVM(FCEUMMRT.FCEUMMCore.Instance, LocalizationService, "SystemNameNES", "ManufacturerNameNintendo", "\uf118", FCEUMMRT.FCEUMMCore.Instance.SupportedExtensions, new string[0]),
                new GameSystemVM(Snes9XRT.Snes9XCore.Instance, LocalizationService, "SystemNameSNES", "ManufacturerNameNintendo", "\uf119", Snes9XRT.Snes9XCore.Instance.SupportedExtensions, new string[0]),
                new GameSystemVM(GambatteRT.GambatteCore.Instance, LocalizationService, "SystemNameGameBoy", "ManufacturerNameNintendo", "\uf11b", GambatteRT.GambatteCore.Instance.SupportedExtensions, new string[0]),
                new GameSystemVM(VBAMRT.VBAMCore.Instance, LocalizationService, "SystemNameGameBoyAdvance", "ManufacturerNameNintendo", "\uf115", VBAMRT.VBAMCore.Instance.SupportedExtensions, new string[0]),
                new GameSystemVM(GPGXRT.GPGXCore.Instance, LocalizationService, "SystemNameSG1000", "ManufacturerNameSega", "\uf102", new HashSet<string>{ ".sg" }, new string[0]),
                new GameSystemVM(GPGXRT.GPGXCore.Instance, LocalizationService, "SystemNameMasterSystem", "ManufacturerNameSega", "\uf118", new HashSet<string>{ ".sms" }, new string[0]),
                new GameSystemVM(GPGXRT.GPGXCore.Instance, LocalizationService, "SystemNameGameGear", "ManufacturerNameSega", "\uf129", new HashSet<string>{ ".gg" }, new string[0]),
                new GameSystemVM(GPGXRT.GPGXCore.Instance, LocalizationService, "SystemNameMegaDrive", "ManufacturerNameSega", "\uf124", new HashSet<string>{ ".mds", ".md", ".smd", ".gen" }, new string[0]),
                new GameSystemVM(GPGXRT.GPGXCore.Instance, LocalizationService, "SystemNameMegaCD", "ManufacturerNameSega", "\uf124", CDImageExtensions, CDImageExtensions),
            };

            fileDependencyImporters = new Lazy<FileImporterVM[]>(() =>
            {
                return AvailableCores.Where(d => d.FileDependencies.Any()).SelectMany(d => d.FileDependencies.Select(e => new { core = d, deps = e }))
                    .Select(d => new FileImporterVM(dialogsService, localizationService, platformService, cryptographyService,
                    new WinRTFolder(d.core.SystemFolder), d.deps.Name, d.deps.Description, d.deps.MD5)).ToArray();
            });
        }

        public GameSystemVM SuggestSystemForFile(IFile file)
        {
            var extension = Path.GetExtension(file.Name);
            return Systems.FirstOrDefault(d => d.SupportedExtensions.Contains(extension));
        }

        public bool CheckRootFolderRequired(GameSystemVM system, IFile file)
        {
            var extension = Path.GetExtension(file.Name);
            return system.MultiFileExtensions.Contains(extension);
        }

        public async Task<bool> CheckDependenciesMetAsync(GameSystemVM system)
        {
            var systemFolder = system.Core.SystemFolder;
            foreach(var i in system.Core.FileDependencies)
            {
                try
                {
                    await systemFolder.GetFileAsync(i.Name);
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> StartGameAsync(GameSystemVM system, IFile file, IFolder rootFolder = null)
        {
            if (CoreRunner == null)
            {
                RootFrame.Navigate(typeof(GamePlayerPage));
            }
            else
            {
                await CoreRunner.UnloadGameAsync();
            }

            StreamProvider?.Dispose();
            IStreamProvider streamProvider;
            string virtualMainFilePath;
            GetStreamProviderAndVirtualPath(system, file, rootFolder, out streamProvider, out virtualMainFilePath);
            StreamProvider = streamProvider;

            //Navigation should cause the player page to load, which in turn should initialize the core runner
            while (CoreRunner == null)
            {
                await Task.Delay(100);
            }

            system.Core.GetFileStream = OnCoreGetFileStream;
            var loadSuccessful = false;
            try
            {
                loadSuccessful = await CoreRunner.LoadGameAsync(system.Core, virtualMainFilePath);
            }
            catch
            {
                await StopGameAsync();
                return false;
            }

            if (loadSuccessful)
            {
                GameStarted(this);
            }
            else
            {
                await StopGameAsync();
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
            StreamProvider?.Dispose();
            StreamProvider = null;
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
                StreamProvider?.Dispose();
                StreamProvider = null;
                RootFrame.GoBack();
                GameRuntimeExceptionOccurred(this, e);
            });
        }

        private Windows.Storage.Streams.IRandomAccessStream OnCoreGetFileStream(string path, Windows.Storage.FileAccessMode fileAccess)
        {
            var accessMode = fileAccess == Windows.Storage.FileAccessMode.Read ? PCLStorage.FileAccess.Read : PCLStorage.FileAccess.ReadAndWrite;
            var output = StreamProvider.GetFileStreamAsync(path, accessMode).Result?.AsRandomAccessStream();
            return output;
        }

        private void GetStreamProviderAndVirtualPath(GameSystemVM system, IFile file, IFolder rootFolder, out IStreamProvider provider, out string mainFileVirtualPath)
        {
            IStreamProvider romProvider;
            if (rootFolder == null)
            {
                mainFileVirtualPath = VFS.RomPath + file.Name;
                romProvider = new SingleFileStreamProvider(mainFileVirtualPath, file);
            }
            else
            {
                mainFileVirtualPath = file.Path.Substring(rootFolder.Path.Length + 1);
                mainFileVirtualPath = VFS.RomPath + mainFileVirtualPath;
                romProvider = new FolderStreamProvider(VFS.RomPath, rootFolder);
            }

            var systemProvider = new FolderStreamProvider(VFS.SystemPath, new WinRTFolder(system.Core.SystemFolder));
            var saveProvider = new FolderStreamProvider(VFS.SavePath, new WinRTFolder(system.Core.SaveGameFolder));
            var combinedProvider = new CombinedStreamProvider(new HashSet<IStreamProvider> { romProvider, systemProvider, saveProvider });
            provider = combinedProvider;
        }
    }
}
