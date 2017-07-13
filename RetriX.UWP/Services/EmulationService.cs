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

        private bool InitializationComplete = false;

        private ICore[] AvailableCores;

        private static readonly string[] archiveExtensions = { ".zip" };
        public IReadOnlyList<string> ArchiveExtensions => archiveExtensions;

        private GameSystemVM[] systems = new GameSystemVM[0];
        public IReadOnlyList<GameSystemVM> Systems => systems;

        private FileImporterVM[] fileDependencyImporters = new FileImporterVM[0];
        public IReadOnlyList<FileImporterVM> FileDependencyImporters => fileDependencyImporters;

        public string GameID => CoreRunner?.GameID;

        public event CoresInitializedDelegate CoresInitialized;
        public event GameStartedDelegate GameStarted;
        public event GameRuntimeExceptionOccurredDelegate GameRuntimeExceptionOccurred;

        public EmulationService(IUserDialogs dialogsService, ILocalizationService localizationService, IPlatformService platformService, ICryptographyService cryptographyService)
        {
            LocalizationService = localizationService;
            PlatformService = platformService;

            RootFrame.Navigated += OnNavigated;

            Task.Run(() =>
            {
                AvailableCores = new ICore[] { BeetlePSXRT.BeetlePSXCore.Instance, FCEUMMRT.FCEUMMCore.Instance, Snes9XRT.Snes9XCore.Instance, GambatteRT.GambatteCore.Instance, VBAMRT.VBAMCore.Instance, GPGXRT.GPGXCore.Instance };

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
                new GameSystemVM(YabauseRT.YabauseCore.Instance, LocalizationService, "SystemNameSaturn", "ManufacturerNameSega", "\uf124", YabauseRT.YabauseCore.Instance.SupportedExtensions, CDImageExtensions),
                new GameSystemVM(BeetlePSXRT.BeetlePSXCore.Instance, LocalizationService, "SystemNamePlayStation", "ManufacturerNameSony", "\uf128", CDImageExtensions, CDImageExtensions),
                };

                fileDependencyImporters = AvailableCores.Where(d => d.FileDependencies.Any()).SelectMany(d => d.FileDependencies.Select(e => new { core = d, deps = e }))
                        .Select(d => new FileImporterVM(dialogsService, localizationService, platformService, cryptographyService,
                        new WinRTFolder(d.core.SystemFolder), d.deps.Name, d.deps.Description, d.deps.MD5)).ToArray();
            }).ContinueWith(d =>
            {
                InitializationComplete = true;
                PlatformService.RunOnUIThreadAsync(() => CoresInitialized(this));
            });
        }

        
        public async Task<GameSystemVM> SuggestSystemForFileAsync(IFile file)
        {
            while (!InitializationComplete)
            {
                await Task.Delay(100);
            }

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
            StreamProvider = null;
            string virtualMainFilePath = null;
            if (!ArchiveExtensions.Contains(Path.GetExtension(file.Name)))
            {
                IStreamProvider streamProvider;
                GetStreamProviderAndVirtualPath(system, file, rootFolder, out streamProvider, out virtualMainFilePath);
                StreamProvider = streamProvider;
            }
            else
            {
                var archiveProvider = new ArchiveStreamProvider(VFS.RomPath, file);
                await archiveProvider.InitializeAsync();
                StreamProvider = archiveProvider;
                var entries = await StreamProvider.ListEntriesAsync();
                virtualMainFilePath = entries.FirstOrDefault(d => system.SupportedExtensions.Contains(Path.GetExtension(d)));
            }

            //Navigation should cause the player page to load, which in turn should initialize the core runner
            while (CoreRunner == null)
            {
                await Task.Delay(100);
            }

            if (virtualMainFilePath == null)
            {
                return false;
            }

            system.Core.OpenFileStream = OnCoreOpenFileStream;
            system.Core.CloseFileStream = OnCoreCloseFileStream;
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

        private Windows.Storage.Streams.IRandomAccessStream OnCoreOpenFileStream(string path, Windows.Storage.FileAccessMode fileAccess)
        {
            var accessMode = fileAccess == Windows.Storage.FileAccessMode.Read ? PCLStorage.FileAccess.Read : PCLStorage.FileAccess.ReadAndWrite;
            var stream = StreamProvider.OpenFileStreamAsync(path, accessMode).Result;
            var output = stream?.AsRandomAccessStream();
            return output;
        }

        private void OnCoreCloseFileStream(Windows.Storage.Streams.IRandomAccessStream stream)
        {
            StreamProvider.CloseStream(stream.AsStream());
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
