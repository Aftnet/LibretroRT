using Acr.UserDialogs;
using LibretroRT;
using LibretroRT.FrontendComponents.Common;
using Plugin.FileSystem.Abstractions;
using RetriX.Shared.Services;
using RetriX.Shared.StreamProviders;
using RetriX.Shared.ViewModels;
using RetriX.UWP.Pages;
using System;
using System.Collections.Generic;
using System.IO;
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
        private static readonly IReadOnlyDictionary<InjectedInputTypes, InputTypes> InjectedInputMapping = new Dictionary<InjectedInputTypes, InputTypes>
        {
            { InjectedInputTypes.DeviceIdJoypadA, InputTypes.DeviceIdJoypadA },
            { InjectedInputTypes.DeviceIdJoypadB, InputTypes.DeviceIdJoypadB },
            { InjectedInputTypes.DeviceIdJoypadDown, InputTypes.DeviceIdJoypadDown },
            { InjectedInputTypes.DeviceIdJoypadLeft, InputTypes.DeviceIdJoypadLeft },
            { InjectedInputTypes.DeviceIdJoypadRight, InputTypes.DeviceIdJoypadRight },
            { InjectedInputTypes.DeviceIdJoypadSelect, InputTypes.DeviceIdJoypadSelect },
            { InjectedInputTypes.DeviceIdJoypadStart, InputTypes.DeviceIdJoypadStart },
            { InjectedInputTypes.DeviceIdJoypadUp, InputTypes.DeviceIdJoypadUp },
            { InjectedInputTypes.DeviceIdJoypadX, InputTypes.DeviceIdJoypadX },
            { InjectedInputTypes.DeviceIdJoypadY, InputTypes.DeviceIdJoypadY },
        };

        private readonly ILocalizationService LocalizationService;
        private readonly IPlatformService PlatformService;
        private readonly IInputManager InputManager;

        private readonly Frame RootFrame = Window.Current.Content as Frame;

        private IStreamProvider StreamProvider;
        private ICoreRunner CoreRunner;

        private bool InitializationComplete = false;

        private static readonly string[] archiveExtensions = { ".zip" };
        public IReadOnlyList<string> ArchiveExtensions => archiveExtensions;

        private ViewModels.GameSystemVM[] systems = new ViewModels.GameSystemVM[0];
        public IReadOnlyList<Shared.ViewModels.GameSystemVM> Systems => systems;

        private FileImporterVM[] fileDependencyImporters = new FileImporterVM[0];
        public IReadOnlyList<FileImporterVM> FileDependencyImporters => fileDependencyImporters;

        public string GameID => CoreRunner?.GameID;

        public event CoresInitializedDelegate CoresInitialized;
        public event GameStartedDelegate GameStarted;
        public event GameRuntimeExceptionOccurredDelegate GameRuntimeExceptionOccurred;

        public EmulationService(IFileSystem fileSystem, IUserDialogs dialogsService, ILocalizationService localizationService, IPlatformService platformService, ICryptographyService cryptographyService, IInputManager inputManager)
        {
            LocalizationService = localizationService;
            PlatformService = platformService;
            InputManager = inputManager;

            RootFrame.Navigated += OnNavigated;

            Task.Run(() =>
            {
                var CDImageExtensions = new HashSet<string> { ".bin", ".cue", ".iso", ".mds", ".mdf" };

                systems = new ViewModels.GameSystemVM[]
                {
                new ViewModels.GameSystemVM(FCEUMMRT.FCEUMMCore.Instance, LocalizationService, "SystemNameNES", "ManufacturerNameNintendo", "\uf118"),
                new ViewModels.GameSystemVM(Snes9XRT.Snes9XCore.Instance, LocalizationService, "SystemNameSNES", "ManufacturerNameNintendo", "\uf119"),
                //new ViewModels.GameSystemVM(ParallelN64RT.ParallelN64Core.Instance, LocalizationService, "SystemNameNintendo64", "ManufacturerNameNintendo", "\uf116"),
                new ViewModels.GameSystemVM(GambatteRT.GambatteCore.Instance, LocalizationService, "SystemNameGameBoy", "ManufacturerNameNintendo", "\uf11b"),
                new ViewModels.GameSystemVM(VBAMRT.VBAMCore.Instance, LocalizationService, "SystemNameGameBoyAdvance", "ManufacturerNameNintendo", "\uf115"),
                //new ViewModels.GameSystemVM(MelonDSRT.MelonDSCore.Instance, LocalizationService, "SystemNameDS", "ManufacturerNameNintendo", "\uf117"),
                new ViewModels.GameSystemVM(GPGXRT.GPGXCore.Instance, LocalizationService, "SystemNameSG1000", "ManufacturerNameSega", "\uf102", true, new HashSet<string>{ ".sg" }),
                new ViewModels.GameSystemVM(GPGXRT.GPGXCore.Instance, LocalizationService, "SystemNameMasterSystem", "ManufacturerNameSega", "\uf118", true, new HashSet<string>{ ".sms" }),
                new ViewModels.GameSystemVM(GPGXRT.GPGXCore.Instance, LocalizationService, "SystemNameGameGear", "ManufacturerNameSega", "\uf129", true, new HashSet<string>{ ".gg" }),
                new ViewModels.GameSystemVM(GPGXRT.GPGXCore.Instance, LocalizationService, "SystemNameMegaDrive", "ManufacturerNameSega", "\uf124", true, new HashSet<string>{ ".mds", ".md", ".smd", ".gen" }),
                new ViewModels.GameSystemVM(GPGXRT.GPGXCore.Instance, LocalizationService, "SystemNameMegaCD", "ManufacturerNameSega", "\uf124", false, new HashSet<string>{ ".bin", ".cue", ".iso" }, CDImageExtensions),
                //new ViewModels.GameSystemVM(BeetleSaturnRT.BeetleSaturnCore.Instance, LocalizationService, "SystemNameSaturn", "ManufacturerNameSega", "\uf124", false, null, CDImageExtensions),
                new ViewModels.GameSystemVM(BeetlePSXRT.BeetlePSXCore.Instance, LocalizationService, "SystemNamePlayStation", "ManufacturerNameSony", "\uf128", false, null, CDImageExtensions),
                new ViewModels.GameSystemVM(BeetlePCEFastRT.BeetlePCEFastCore.Instance, LocalizationService, "SystemNamePCEngine", "ManufacturerNameNEC", "\uf124", true, new HashSet<string>{ ".pce" }),
                new ViewModels.GameSystemVM(BeetlePCEFastRT.BeetlePCEFastCore.Instance, LocalizationService, "SystemNamePCEngineCD", "ManufacturerNameNEC", "\uf124", false, new HashSet<string>{ ".cue", ".ccd" }, CDImageExtensions),
                new ViewModels.GameSystemVM(BeetlePCFXRT.BeetlePCFXCore.Instance, LocalizationService, "SystemNamePCFX", "ManufacturerNameNEC", "\uf124", false, new HashSet<string>{ ".cue", ".ccd", ".toc" }, CDImageExtensions),
                new ViewModels.GameSystemVM(BeetleWswanRT.BeetleWswanCore.Instance, LocalizationService, "SystemNameWonderSwan", "ManufacturerNameBandai", "\uf129"),
                //new ViewModels.GameSystemVM(FBAlphaRT.FBAlphaCore.Instance, LocalizationService, "SystemNameNeoGeo", "ManufacturerNameSNK", "\uf102", false),
                new ViewModels.GameSystemVM(BeetleNGPRT.BeetleNGPCore.Instance, LocalizationService, "SystemNameNeoGeoPocket", "ManufacturerNameSNK", "\uf129"),
                //new ViewModels.GameSystemVM(FBAlphaRT.FBAlphaCore.Instance, LocalizationService, "SystemNameArcade", "ManufacturerNameFBAlpha", "\uf102", true),
                };

                var allCores = systems.Select(d => d.Core).Distinct().ToArray();
                fileDependencyImporters = allCores.Where(d => d.FileDependencies.Any()).SelectMany(d => d.FileDependencies.Select(e => new { core = d, deps = e }))
                        .Select(d => new FileImporterVM(fileSystem, dialogsService, localizationService, platformService, cryptographyService,
                        new Plugin.FileSystem.DirectoryInfo((StorageFolder)d.core.SystemFolder), d.deps.Name, d.deps.Description, d.deps.MD5)).ToArray();
            }).ContinueWith(d =>
            {
                InitializationComplete = true;
                PlatformService.RunOnUIThreadAsync(() => CoresInitialized(this));
            });
        }

        public async Task<Shared.ViewModels.GameSystemVM> SuggestSystemForFileAsync(IFileInfo file)
        {
            while (!InitializationComplete)
            {
                await Task.Delay(100);
            }

            var extension = Path.GetExtension(file.Name);
            return Systems.FirstOrDefault(d => d.SupportedExtensions.Contains(extension));
        }

        public async Task<bool> StartGameAsync(Shared.ViewModels.GameSystemVM system, IFileInfo file, IDirectoryInfo rootFolder = null)
        {
            ViewModels.GameSystemVM nativeSystem = (ViewModels.GameSystemVM)system;

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
            if (nativeSystem.Core.NativeArchiveSupport || !ArchiveExtensions.Contains(Path.GetExtension(file.Name)))
            {
                IStreamProvider streamProvider;
                GetStreamProviderAndVirtualPath(nativeSystem, file, rootFolder, out streamProvider, out virtualMainFilePath);
                StreamProvider = streamProvider;
            }
            else
            {
                var archiveProvider = new ArchiveStreamProvider(VFS.RomPath, file);
                await archiveProvider.InitializeAsync();
                StreamProvider = archiveProvider;
                var entries = await StreamProvider.ListEntriesAsync();
                virtualMainFilePath = entries.FirstOrDefault(d => nativeSystem.SupportedExtensions.Contains(Path.GetExtension(d)));
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

            nativeSystem.Core.OpenFileStream = OnCoreOpenFileStream;
            nativeSystem.Core.CloseFileStream = OnCoreCloseFileStream;
            var loadSuccessful = false;
            try
            {
                loadSuccessful = await CoreRunner.LoadGameAsync(nativeSystem.Core, virtualMainFilePath);
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

        public void InjectInputPlayer1(InjectedInputTypes inputType)
        {
            InputManager.InjectInputPlayer1(InjectedInputMapping[inputType]);
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
            var accessMode = fileAccess == Windows.Storage.FileAccessMode.Read ? FileAccess.Read : FileAccess.ReadWrite;
            var stream = StreamProvider.OpenFileStreamAsync(path, accessMode).Result;
            var output = stream?.AsRandomAccessStream();
            return output;
        }

        private void OnCoreCloseFileStream(Windows.Storage.Streams.IRandomAccessStream stream)
        {
            StreamProvider.CloseStream(stream.AsStream());
        }

        private void GetStreamProviderAndVirtualPath(ViewModels.GameSystemVM system, IFileInfo file, IDirectoryInfo rootFolder, out IStreamProvider provider, out string mainFileVirtualPath)
        {
            IStreamProvider romProvider;
            if (rootFolder == null)
            {
                mainFileVirtualPath = $"{VFS.RomPath}{Path.DirectorySeparatorChar}{file.Name}";
                romProvider = new SingleFileStreamProvider(mainFileVirtualPath, file);
            }
            else
            {
                mainFileVirtualPath = file.FullName.Substring(rootFolder.FullName.Length + 1);
                mainFileVirtualPath = $"{VFS.RomPath}{Path.DirectorySeparatorChar}{mainFileVirtualPath}";
                romProvider = new FolderStreamProvider(VFS.RomPath, rootFolder);
            }

            var systemProvider = new FolderStreamProvider(VFS.SystemPath, new Plugin.FileSystem.DirectoryInfo((StorageFolder)system.Core.SystemFolder));
            var saveProvider = new FolderStreamProvider(VFS.SavePath, new Plugin.FileSystem.DirectoryInfo((StorageFolder)system.Core.SaveGameFolder));
            var combinedProvider = new CombinedStreamProvider(new HashSet<IStreamProvider> { romProvider, systemProvider, saveProvider });
            provider = combinedProvider;
        }
    }
}
