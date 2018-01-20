using Acr.UserDialogs;
using LibRetriX;
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RetriX.UWP.Services
{
    public class EmulationService : IEmulationService
    {
        private const string VFSRomPath = "ROM\\";
        private const string VFSSystemPath = "System\\";
        private const string VFSSavePath = "Save\\";

        private static readonly Type GamePlayerPageType = typeof(GamePlayerPage);

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

        private readonly IFileSystem FileSystem;
        private readonly ILocalizationService LocalizationService;
        private readonly IPlatformService PlatformService;
        private readonly IInputManager InputManager;

        private readonly Frame RootFrame = Window.Current.Content as Frame;

        private IStreamProvider StreamProvider;
        private ICoreRunner CoreRunner;

        private static readonly string[] archiveExtensions = { ".zip" };
        public IReadOnlyList<string> ArchiveExtensions => archiveExtensions;

        private GameSystemVM[] systems = new GameSystemVM[0];
        public IReadOnlyList<GameSystemVM> Systems => systems;

        public IReadOnlyList<FileImporterVM> FileDependencyImporters { get; private set; }

        public string GameID => CoreRunner?.GameID;

        public event GameStartedDelegate GameStarted;
        public event GameStoppedDelegate GameStopped;
        public event GameRuntimeExceptionOccurredDelegate GameRuntimeExceptionOccurred;

        public EmulationService(IFileSystem fileSystem, IUserDialogs dialogsService, ILocalizationService localizationService, IPlatformService platformService, ICryptographyService cryptographyService, IInputManager inputManager)
        {
            FileSystem = fileSystem;
            LocalizationService = localizationService;
            PlatformService = platformService;
            InputManager = inputManager;

            RootFrame.Navigated += OnNavigated;

            var CDImageExtensions = new HashSet<string> { ".bin", ".cue", ".iso", ".mds", ".mdf" };

            systems = new GameSystemVM[]
            {
                //new ViewModels.GameSystemVM(FCEUMMRT.FCEUMMCore.Instance, LocalizationService, "SystemNameNES", "ManufacturerNameNintendo", "\uf118"),
                //new ViewModels.GameSystemVM(Snes9XRT.Snes9XCore.Instance, LocalizationService, "SystemNameSNES", "ManufacturerNameNintendo", "\uf119"),
                //new ViewModels.GameSystemVM(ParallelN64RT.ParallelN64Core.Instance, LocalizationService, "SystemNameNintendo64", "ManufacturerNameNintendo", "\uf116"),
                //new ViewModels.GameSystemVM(GambatteRT.GambatteCore.Instance, LocalizationService, "SystemNameGameBoy", "ManufacturerNameNintendo", "\uf11b"),
                //new ViewModels.GameSystemVM(VBAMRT.VBAMCore.Instance, LocalizationService, "SystemNameGameBoyAdvance", "ManufacturerNameNintendo", "\uf115"),
                //new ViewModels.GameSystemVM(MelonDSRT.MelonDSCore.Instance, LocalizationService, "SystemNameDS", "ManufacturerNameNintendo", "\uf117"),
                new GameSystemVM(LibRetriX.GPGX.Core.Instance, FileSystem, LocalizationService, "SystemNameSG1000", "ManufacturerNameSega", "\uf102", true, new HashSet<string>{ ".sg" }),
                new GameSystemVM(LibRetriX.GPGX.Core.Instance, FileSystem, LocalizationService, "SystemNameMasterSystem", "ManufacturerNameSega", "\uf118", true, new HashSet<string>{ ".sms" }),
                new GameSystemVM(LibRetriX.GPGX.Core.Instance, FileSystem, LocalizationService, "SystemNameGameGear", "ManufacturerNameSega", "\uf129", true, new HashSet<string>{ ".gg" }),
                new GameSystemVM(LibRetriX.GPGX.Core.Instance, FileSystem, LocalizationService, "SystemNameMegaDrive", "ManufacturerNameSega", "\uf124", true, new HashSet<string>{ ".mds", ".md", ".smd", ".gen" }),
                new GameSystemVM(LibRetriX.GPGX.Core.Instance, FileSystem, LocalizationService, "SystemNameMegaCD", "ManufacturerNameSega", "\uf124", false, new HashSet<string>{ ".bin", ".cue", ".iso", ".chd" }, CDImageExtensions),
                //new ViewModels.GameSystemVM(BeetleSaturnRT.BeetleSaturnCore.Instance, LocalizationService, "SystemNameSaturn", "ManufacturerNameSega", "\uf124", false, null, CDImageExtensions),
                //new ViewModels.GameSystemVM(BeetlePSXRT.BeetlePSXCore.Instance, LocalizationService, "SystemNamePlayStation", "ManufacturerNameSony", "\uf128", false, null, CDImageExtensions),
                //new ViewModels.GameSystemVM(BeetlePCEFastRT.BeetlePCEFastCore.Instance, LocalizationService, "SystemNamePCEngine", "ManufacturerNameNEC", "\uf124", true, new HashSet<string>{ ".pce" }),
                //new ViewModels.GameSystemVM(BeetlePCEFastRT.BeetlePCEFastCore.Instance, LocalizationService, "SystemNamePCEngineCD", "ManufacturerNameNEC", "\uf124", false, new HashSet<string>{ ".cue", ".ccd", ".chd" }, CDImageExtensions),
                //new ViewModels.GameSystemVM(BeetlePCFXRT.BeetlePCFXCore.Instance, LocalizationService, "SystemNamePCFX", "ManufacturerNameNEC", "\uf124", false, null, CDImageExtensions),
                //new ViewModels.GameSystemVM(BeetleWswanRT.BeetleWswanCore.Instance, LocalizationService, "SystemNameWonderSwan", "ManufacturerNameBandai", "\uf129"),
                //new ViewModels.GameSystemVM(FBAlphaRT.FBAlphaCore.Instance, LocalizationService, "SystemNameNeoGeo", "ManufacturerNameSNK", "\uf102", false),
                //new ViewModels.GameSystemVM(BeetleNGPRT.BeetleNGPCore.Instance, LocalizationService, "SystemNameNeoGeoPocket", "ManufacturerNameSNK", "\uf129"),
                //new ViewModels.GameSystemVM(FBAlphaRT.FBAlphaCore.Instance, LocalizationService, "SystemNameArcade", "ManufacturerNameFBAlpha", "\uf102", true),
            };

            Task.Run(() => GetFileDependencyImportersAsync(systems, fileSystem, dialogsService, localizationService, platformService, cryptographyService)).ContinueWith(d =>
            {
                FileDependencyImporters = d.Result;
            });
        }

        public IEnumerable<GameSystemVM> FilterSystemsForFile(IFileInfo file)
        {
            var extension = Path.GetExtension(file.Name);
            return Systems.Where(d => d.SupportedExtensions.Contains(extension));
        }

        public async Task<bool> StartGameAsync(GameSystemVM system, IFileInfo file, IDirectoryInfo rootFolder = null)
        {
            if (CoreRunner == null)
            {
                RootFrame.Navigate(GamePlayerPageType);
            }
            else
            {
                await CoreRunner.UnloadGameAsync();
            }

            StreamProvider?.Dispose();
            StreamProvider = null;
            string virtualMainFilePath = null;
            if (system.Core.NativeArchiveSupport || !ArchiveExtensions.Contains(Path.GetExtension(file.Name)))
            {
                virtualMainFilePath = $"{VFSRomPath}{Path.DirectorySeparatorChar}{file.Name}";
                StreamProvider = new SingleFileStreamProvider(virtualMainFilePath, file);
                if (rootFolder != null)
                {
                    virtualMainFilePath = file.FullName.Substring(rootFolder.FullName.Length + 1);
                    virtualMainFilePath = $"{VFSRomPath}{Path.DirectorySeparatorChar}{virtualMainFilePath}";
                    StreamProvider = new FolderStreamProvider(VFSRomPath, rootFolder);
                }
            }
            else
            {
                var archiveProvider = new ArchiveStreamProvider(VFSRomPath, file);
                await archiveProvider.InitializeAsync();
                StreamProvider = archiveProvider;
                var entries = await StreamProvider.ListEntriesAsync();
                virtualMainFilePath = entries.FirstOrDefault(d => system.SupportedExtensions.Contains(Path.GetExtension(d)));
            }

            var systemFolder = await system.GetSystemDirectoryAsync();
            var systemProvider = new FolderStreamProvider(VFSSystemPath, systemFolder);
            var saveFolder = await system.GetSaveDirectoryAsync();
            var saveProvider = new FolderStreamProvider(VFSSavePath, saveFolder);
            StreamProvider = new CombinedStreamProvider(new HashSet<IStreamProvider>() { StreamProvider, systemProvider, saveProvider });

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
                GameStarted?.Invoke(this);
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
            return CoreRunner != null ? Task.CompletedTask : CoreRunner.ResetGameAsync();
        }

        public async Task StopGameAsync()
        {
            if (CoreRunner != null)
            {
                await CoreRunner.UnloadGameAsync();
            }

            CleanupAndGoBack();
            GameStopped?.Invoke(this);
        }

        public Task PauseGameAsync()
        {
            return CoreRunner != null ? CoreRunner.PauseCoreExecutionAsync() : Task.CompletedTask;
        }

        public Task ResumeGameAsync()
        {
            return CoreRunner != null ? CoreRunner.ResumeCoreExecutionAsync() : Task.CompletedTask;
        }

        public Task<bool> SaveGameStateAsync(Stream outputStream)
        {
            if (CoreRunner == null)
            {
                return Task.FromResult(false);
            }

            var output = new byte[CoreRunner.SerializationSize];
            return CoreRunner.SaveGameStateAsync(outputStream);
        }

        public Task<bool> LoadGameStateAsync(Stream inputStream)
        {
            if (CoreRunner == null)
            {
                return Task.FromResult(false);
            }

            return CoreRunner.LoadGameStateAsync(inputStream);
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
                CleanupAndGoBack();
                GameRuntimeExceptionOccurred?.Invoke(this, e);
            });
        }

        private Stream OnCoreOpenFileStream(string path, FileAccess fileAccess)
        {
            var stream = StreamProvider.OpenFileStreamAsync(path, fileAccess).Result;
            return stream;
        }

        private void OnCoreCloseFileStream(Stream stream)
        {
            StreamProvider.CloseStream(stream);
        }

        private string GetVirtualGamePath(IFileInfo file, IDirectoryInfo rootFolder)
        {
            var mainFileVirtualPath = $"{VFSRomPath}{Path.DirectorySeparatorChar}{file.Name}";
            if (rootFolder != null)
            {
                mainFileVirtualPath = file.FullName.Substring(rootFolder.FullName.Length + 1);
                mainFileVirtualPath = $"{VFSRomPath}{Path.DirectorySeparatorChar}{mainFileVirtualPath}";
            }

            return mainFileVirtualPath;
        }

        private void CleanupAndGoBack()
        {
            StreamProvider?.Dispose();
            StreamProvider = null;

            if (RootFrame.CurrentSourcePageType == GamePlayerPageType)
            {
                RootFrame.GoBack();
            }

            PlatformService.ChangeMousePointerVisibility(MousePointerVisibility.Visible);
        }

        private static async Task<List<FileImporterVM>> GetFileDependencyImportersAsync(IEnumerable<GameSystemVM> gameSystems, IFileSystem fileSystem, IUserDialogs dialogsService, ILocalizationService localizationService, IPlatformService platformService, ICryptographyService cryptographyService)
        {
            var importers = new List<FileImporterVM>();
            var distinctCores = new HashSet<ICore>();
            foreach (var i in gameSystems)
            {
                var core = i.Core;
                if (distinctCores.Contains(core))
                {
                    continue;
                }

                distinctCores.Add(core);
                var systemFolder = await i.GetSystemDirectoryAsync();
                foreach (var j in core.FileDependencies)
                {
                    importers.Add(new FileImporterVM(fileSystem, dialogsService, localizationService, platformService, cryptographyService, systemFolder, j.Name, j.Description, j.MD5));
                }
            }

            return importers;
        }
    }
}
