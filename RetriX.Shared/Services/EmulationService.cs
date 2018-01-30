using Acr.UserDialogs;
using GalaSoft.MvvmLight.Views;
using LibRetriX;
using Plugin.FileSystem.Abstractions;
using Plugin.LocalNotifications.Abstractions;
using RetriX.Shared.Services;
using RetriX.Shared.StreamProviders;
using RetriX.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RetriX.UWP.Services
{
    public class EmulationService : IEmulationService
    {
        public const string StateSavedToSlotMessageTitleKey = "StateSavedToSlotMessageTitleKey";
        public const string StateSavedToSlotMessageBodyKey = "StateSavedToSlotMessageBodyKey";

        private const string VFSRomPath = "ROM";
        private const string VFSSystemPath = "System";
        private const string VFSSavePath = "Save";

        private const string GamePlayerPageKey = nameof(GamePlayerVM);

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

        private readonly INavigationService NavigationService;
        private readonly IFileSystem FileSystem;
        private readonly ILocalizationService LocalizationService;
        private readonly IPlatformService PlatformService;
        private readonly ISaveStateService SaveStateService;
        private readonly ILocalNotifications NotificationService;

        private readonly IVideoService VideoService;
        private readonly IAudioService AudioService;
        private readonly IInputService InputService;

        private bool CorePaused;

        private readonly SemaphoreSlim CoreSemaphore = new SemaphoreSlim(1, 1);

        private ICore core;
        private ICore Core
        {
            get => core;
            set
            {
                if (core == value)
                {
                    return;
                }

                if (core != null)
                {
                    Core.GeometryChanged -= VideoService.GeometryChanged;
                    Core.PixelFormatChanged -= VideoService.PixelFormatChanged;
                    Core.RenderVideoFrame -= VideoService.RenderVideoFrame;
                    Core.TimingsChanged -= VideoService.TimingsChanged;
                    Core.RotationChanged -= VideoService.RotationChanged;
                    Core.TimingsChanged -= AudioService.TimingChanged;
                    Core.RenderAudioFrames -= AudioService.RenderAudioFrames;
                    Core.PollInput = null;
                    Core.GetInputState = null;
                    Core.OpenFileStream = null;
                    Core.CloseFileStream = null;
                }

                core = value;

                if (core != null)
                {
                    Core.GeometryChanged += VideoService.GeometryChanged;
                    Core.PixelFormatChanged += VideoService.PixelFormatChanged;
                    Core.RenderVideoFrame += VideoService.RenderVideoFrame;
                    Core.TimingsChanged += VideoService.TimingsChanged;
                    Core.RotationChanged += VideoService.RotationChanged;
                    Core.TimingsChanged += AudioService.TimingChanged;
                    Core.RenderAudioFrames += AudioService.RenderAudioFrames;
                    Core.PollInput = InputService.PollInput;
                    Core.GetInputState = InputService.GetInputState;
                    Core.OpenFileStream = OnCoreOpenFileStream;
                    Core.CloseFileStream = OnCoreCloseFileStream;
                }
            }
        }

        private IStreamProvider streamProvider;
        private IStreamProvider StreamProvider
        {
            get => streamProvider;
            set { if (streamProvider != value) streamProvider?.Dispose(); streamProvider = value; }
        }
        
        private static readonly string[] archiveExtensions = { ".zip" };
        public IReadOnlyList<string> ArchiveExtensions => archiveExtensions;

        private readonly GameSystemVM[] systems;
        public IReadOnlyList<GameSystemVM> Systems => systems;

        public event GameStartedDelegate GameStarted;
        public event GameStoppedDelegate GameStopped;
        public event GameRuntimeExceptionOccurredDelegate GameRuntimeExceptionOccurred;

        public EmulationService(INavigationService navigationService,  IFileSystem fileSystem, IUserDialogs dialogsService,
            ILocalizationService localizationService, IPlatformService platformService, ISaveStateService saveStateService,
            ILocalNotifications notificationService, ICryptographyService cryptographyService,
            IVideoService videoService, IAudioService audioService, IInputService inputService)
        {
            NavigationService = navigationService;
            FileSystem = fileSystem;
            LocalizationService = localizationService;
            PlatformService = platformService;
            SaveStateService = saveStateService;
            NotificationService = notificationService;

            VideoService = videoService;
            AudioService = audioService;
            InputService = inputService;

            VideoService.RequestRunCoreFrame += d => OnCoreRunFrameRequested();

            var CDImageExtensions = new HashSet<string> { ".bin", ".cue", ".iso", ".mds", ".mdf" };

            systems = new GameSystemVM[]
            {
                new GameSystemVM(LibRetriX.FCEUMM.Core.Instance, FileSystem, LocalizationService, "SystemNameNES", "ManufacturerNameNintendo", "\uf118"),
                new GameSystemVM(LibRetriX.Snes9X.Core.Instance, FileSystem, LocalizationService, "SystemNameSNES", "ManufacturerNameNintendo", "\uf119"),
                //new GameSystemVM(LibRetriX.ParallelN64.Core.Instance, FileSystem, LocalizationService, "SystemNameNintendo64", "ManufacturerNameNintendo", "\uf116"),
                new GameSystemVM(LibRetriX.Gambatte.Core.Instance, FileSystem, LocalizationService, "SystemNameGameBoy", "ManufacturerNameNintendo", "\uf11b"),
                new GameSystemVM(LibRetriX.VBAM.Core.Instance, FileSystem, LocalizationService, "SystemNameGameBoyAdvance", "ManufacturerNameNintendo", "\uf115"),
                new GameSystemVM(LibRetriX.MelonDS.Core.Instance, FileSystem, LocalizationService, "SystemNameDS", "ManufacturerNameNintendo", "\uf117"),
                new GameSystemVM(LibRetriX.GPGX.Core.Instance, FileSystem, LocalizationService, "SystemNameSG1000", "ManufacturerNameSega", "\uf102", true, new HashSet<string> { ".sg" }),
                new GameSystemVM(LibRetriX.GPGX.Core.Instance, FileSystem, LocalizationService, "SystemNameMasterSystem", "ManufacturerNameSega", "\uf118", true, new HashSet<string> { ".sms" }),
                new GameSystemVM(LibRetriX.GPGX.Core.Instance, FileSystem, LocalizationService, "SystemNameGameGear", "ManufacturerNameSega", "\uf129", true, new HashSet<string> { ".gg" }),
                new GameSystemVM(LibRetriX.GPGX.Core.Instance, FileSystem, LocalizationService, "SystemNameMegaDrive", "ManufacturerNameSega", "\uf124", true, new HashSet<string> { ".mds", ".md", ".smd", ".gen" }),
                new GameSystemVM(LibRetriX.GPGX.Core.Instance, FileSystem, LocalizationService, "SystemNameMegaCD", "ManufacturerNameSega", "\uf124", false, new HashSet<string> { ".bin", ".cue", ".iso", ".chd" }, CDImageExtensions),
                //new GameSystemVM(LibRetriX.BeetleSaturn.Core.Instance, FileSystem, LocalizationService, "SystemNameSaturn", "ManufacturerNameSega", "\uf124", false, null, CDImageExtensions),
                new GameSystemVM(LibRetriX.BeetlePSX.Core.Instance, FileSystem, LocalizationService, "SystemNamePlayStation", "ManufacturerNameSony", "\uf128", false, null, CDImageExtensions),
                new GameSystemVM(LibRetriX.BeetlePCEFast.Core.Instance, FileSystem, LocalizationService, "SystemNamePCEngine", "ManufacturerNameNEC", "\uf124", true, new HashSet<string> { ".pce" }),
                new GameSystemVM(LibRetriX.BeetlePCEFast.Core.Instance, FileSystem, LocalizationService, "SystemNamePCEngineCD", "ManufacturerNameNEC", "\uf124", false, new HashSet<string> { ".cue", ".ccd", ".chd" }, CDImageExtensions),
                new GameSystemVM(LibRetriX.BeetlePCFX.Core.Instance, FileSystem, LocalizationService, "SystemNamePCFX", "ManufacturerNameNEC", "\uf124", false, null, CDImageExtensions),
                new GameSystemVM(LibRetriX.BeetleWswan.Core.Instance, FileSystem, LocalizationService, "SystemNameWonderSwan", "ManufacturerNameBandai", "\uf129"),
                new GameSystemVM(LibRetriX.FBAlpha.Core.Instance, FileSystem, LocalizationService, "SystemNameNeoGeo", "ManufacturerNameSNK", "\uf102", false),
                new GameSystemVM(LibRetriX.BeetleNGP.Core.Instance, FileSystem, LocalizationService, "SystemNameNeoGeoPocket", "ManufacturerNameSNK", "\uf129"),
                new GameSystemVM(LibRetriX.FBAlpha.Core.Instance, FileSystem, LocalizationService, "SystemNameArcade", "ManufacturerNameFBAlpha", "\uf102", true),
            };
        }

        public async Task<bool> StartGameAsync(GameSystemVM system, IFileInfo file, IDirectoryInfo rootFolder)
        {
            var gameLaunchEnvironment = await GenerateGameLaunchEnvironmentAsync(system, file, rootFolder);
            var provider = gameLaunchEnvironment.Item1;
            var virtualMainFilePath = gameLaunchEnvironment.Item2;

            if (NavigationService.CurrentPageKey != GamePlayerPageKey)
            {
                NavigationService.NavigateTo(GamePlayerPageKey);
            }

            var initTasks = new Task[] { InputService.InitAsync(), AudioService.InitAsync(), VideoService.InitAsync() };
            await Task.WhenAll(initTasks);

            var loadSuccessful = false;
            await CoreSemaphore.WaitAsync();
            try
            {
                if (Core != null)
                {
                    await Task.Run(() => Core.UnloadGame());
                }

                StreamProvider = provider;
                SaveStateService.SetGameId(virtualMainFilePath);
                Core = core;

                loadSuccessful = await Task.Run(() =>
                {
                    try
                    {
                        return Core.LoadGame(virtualMainFilePath);
                    }
                    catch
                    {
                        return false;
                    }
                });

                if (!loadSuccessful)
                {
                    await StopGameAsyncInternal();
                    return loadSuccessful;
                }
            }
            finally
            {
                CoreSemaphore.Release();
            }

            GameStarted?.Invoke(this);
            return loadSuccessful;
        }

        public async Task ResetGameAsync()
        {
            if (Core == null)
            {
                return;
            }

            await CoreSemaphore.WaitAsync();
            try
            {
                await Task.Run(() => Core.Reset());
            }
            finally
            {
                CoreSemaphore.Release();
            }
        }

        public async Task StopGameAsync()
        {
            await CoreSemaphore.WaitAsync();
            try
            {
                await StopGameAsyncInternal();
            }
            finally
            {
                CoreSemaphore.Release();
            }

            GameStopped?.Invoke(this);
        }

        private async Task StopGameAsyncInternal()
        {
            await Task.Run(() => Core.UnloadGame());
            Core = null;

            SaveStateService.SetGameId(null);
            StreamProvider = null;

            if (NavigationService.CurrentPageKey == GamePlayerPageKey)
            {
                NavigationService.GoBack();
            }
        }

        public Task PauseGameAsync()
        {
            return SetCorePaused(true);
        }

        public Task ResumeGameAsync()
        {
            return SetCorePaused(false);
        }

        private async Task SetCorePaused(bool value)
        {
            await CoreSemaphore.WaitAsync();
            if (value)
            {
                await Task.Run(() => AudioService.Stop());
            }

            CorePaused = value;
            CoreSemaphore.Release();
        }

        public async Task<bool> SaveGameStateAsync(uint slotID)
        {
            var success = false;
            if (Core == null)
            {
                return success;
            }

            using (var stream = await SaveStateService.GetStreamForSlotAsync(slotID, FileAccess.ReadWrite))
            {
                if (stream == null)
                {
                    return success;
                }

                await CoreSemaphore.WaitAsync();
                try
                {
                    success = await Task.Run(() => Core.SaveState(stream));
                }
                finally
                {
                    CoreSemaphore.Release();
                }

                await stream.FlushAsync();
            }

            if (success)
            {
                var notificationTitle = LocalizationService.GetLocalizedString(StateSavedToSlotMessageTitleKey);
                var notificationBody = string.Format(LocalizationService.GetLocalizedString(StateSavedToSlotMessageBodyKey), slotID);
                NotificationService.Show(notificationTitle, notificationBody);
            }

            return success;
        }

        public async Task<bool> LoadGameStateAsync(uint slotID)
        {
            var success = false;
            if (Core == null)
            {
                return success;
            }

            using (var stream = await SaveStateService.GetStreamForSlotAsync(slotID, FileAccess.Read))
            {
                if (stream == null)
                {
                    return false;
                }

                await CoreSemaphore.WaitAsync();
                try
                {
                    success = await Task.Run(() => Core.LoadState(stream));
                }
                finally
                {
                    CoreSemaphore.Release();
                }
            }

            return success;
        }

        public void InjectInputPlayer1(InjectedInputTypes inputType)
        {
            InputService.InjectInputPlayer1(InjectedInputMapping[inputType]);
        }

        //Synhronous since it's going to be called by a non UI thread
        private void OnCoreRunFrameRequested()
        {
            if (Core == null || CorePaused || AudioService.ShouldDelayNextFrame)
            {
                return;
            }

            CoreSemaphore.WaitAsync().Wait();
            try
            {
                Core.RunFrame();
            }
            finally
            {
                CoreSemaphore.Release();
            }
        }

        private void OnCoreExceptionOccurred(ICore core, Exception e)
        {
            var task = PlatformService.RunOnUIThreadAsync(async () =>
            {
                await StopGameAsync();
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

        private async Task<Tuple<IStreamProvider, string>> GenerateGameLaunchEnvironmentAsync(GameSystemVM system, IFileInfo file, IDirectoryInfo rootFolder)
        {
            var core = system.Core;

            string virtualMainFilePath = null;
            var provider = default(IStreamProvider);

            if (core.NativeArchiveSupport || !ArchiveExtensions.Contains(Path.GetExtension(file.Name)))
            {
                virtualMainFilePath = $"{VFSRomPath}{Path.DirectorySeparatorChar}{file.Name}";
                provider = new SingleFileStreamProvider(virtualMainFilePath, file);
                if (rootFolder != null)
                {
                    virtualMainFilePath = file.FullName.Substring(rootFolder.FullName.Length + 1);
                    virtualMainFilePath = $"{VFSRomPath}{Path.DirectorySeparatorChar}{virtualMainFilePath}";
                    provider = new FolderStreamProvider(VFSRomPath, rootFolder);
                }
            }
            else
            {
                var archiveProvider = new ArchiveStreamProvider(VFSRomPath, file);
                await archiveProvider.InitializeAsync();
                provider = archiveProvider;
                var entries = await provider.ListEntriesAsync();
                virtualMainFilePath = entries.FirstOrDefault(d => system.SupportedExtensions.Contains(Path.GetExtension(d)));
            }

            var systemFolder = await system.GetSystemDirectoryAsync();
            var systemProvider = new FolderStreamProvider(VFSSystemPath, systemFolder);
            core.SystemRootPath = VFSSystemPath;
            var saveFolder = await system.GetSaveDirectoryAsync();
            var saveProvider = new FolderStreamProvider(VFSSavePath, saveFolder);
            core.SaveRootPath = VFSSavePath;

            provider = new CombinedStreamProvider(new HashSet<IStreamProvider>() { provider, systemProvider, saveProvider });

            return Tuple.Create(provider, virtualMainFilePath);
        }
    }
}
