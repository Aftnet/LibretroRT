using Acr.UserDialogs;
using GalaSoft.MvvmLight.Views;
using LibRetriX;
using Plugin.FileSystem.Abstractions;
using Plugin.LocalNotifications.Abstractions;
using RetriX.Shared.StreamProviders;
using RetriX.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RetriX.Shared.Services
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
        private readonly IPlatformService PlatformService;
        private readonly ISaveStateService SaveStateService;
        private readonly ILocalNotifications NotificationService;

        private readonly IVideoService VideoService;
        private readonly IAudioService AudioService;
        private readonly IInputService InputService;

        private bool CorePaused = false;
        private bool StartStopOperationInProgress = false;

        private readonly SemaphoreSlim CoreSemaphore = new SemaphoreSlim(1, 1);

        private ICore currentCore;
        private ICore CurrentCore
        {
            get => currentCore;
            set
            {
                if (currentCore == value)
                {
                    return;
                }

                if (currentCore != null)
                {
                    currentCore.GeometryChanged -= VideoService.GeometryChanged;
                    currentCore.PixelFormatChanged -= VideoService.PixelFormatChanged;
                    currentCore.RenderVideoFrame -= VideoService.RenderVideoFrame;
                    currentCore.TimingsChanged -= VideoService.TimingsChanged;
                    currentCore.RotationChanged -= VideoService.RotationChanged;
                    currentCore.TimingsChanged -= AudioService.TimingChanged;
                    currentCore.RenderAudioFrames -= AudioService.RenderAudioFrames;
                    currentCore.PollInput = null;
                    currentCore.GetInputState = null;
                    currentCore.OpenFileStream = null;
                    currentCore.CloseFileStream = null;
                }

                currentCore = value;

                if (currentCore != null)
                {
                    currentCore.GeometryChanged += VideoService.GeometryChanged;
                    currentCore.PixelFormatChanged += VideoService.PixelFormatChanged;
                    currentCore.RenderVideoFrame += VideoService.RenderVideoFrame;
                    currentCore.TimingsChanged += VideoService.TimingsChanged;
                    currentCore.RotationChanged += VideoService.RotationChanged;
                    currentCore.TimingsChanged += AudioService.TimingChanged;
                    currentCore.RenderAudioFrames += AudioService.RenderAudioFrames;
                    currentCore.PollInput = InputService.PollInput;
                    currentCore.GetInputState = InputService.GetInputState;
                    currentCore.OpenFileStream = OnCoreOpenFileStream;
                    currentCore.CloseFileStream = OnCoreCloseFileStream;
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
                new GameSystemVM(LibRetriX.FCEUMM.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameNES"), localizationService.GetLocalizedString("ManufacturerNameNintendo"), "\uf118"),
                new GameSystemVM(LibRetriX.Snes9X.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameSNES"), localizationService.GetLocalizedString("ManufacturerNameNintendo"), "\uf119"),
                //new GameSystemVM(LibRetriX.ParallelN64.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameNintendo64"), localizationService.GetLocalizedString("ManufacturerNameNintendo"), "\uf116"),
                new GameSystemVM(LibRetriX.Gambatte.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameGameBoy"), localizationService.GetLocalizedString("ManufacturerNameNintendo"), "\uf11b"),
                new GameSystemVM(LibRetriX.VBAM.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameGameBoyAdvance"), localizationService.GetLocalizedString("ManufacturerNameNintendo"), "\uf115"),
                new GameSystemVM(LibRetriX.MelonDS.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameDS"), localizationService.GetLocalizedString("ManufacturerNameNintendo"), "\uf117"),
                new GameSystemVM(LibRetriX.GPGX.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameSG1000"), localizationService.GetLocalizedString("ManufacturerNameSega"), "\uf102", true, new HashSet<string> { ".sg" }),
                new GameSystemVM(LibRetriX.GPGX.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameMasterSystem"), localizationService.GetLocalizedString("ManufacturerNameSega"), "\uf118", true, new HashSet<string> { ".sms" }),
                new GameSystemVM(LibRetriX.GPGX.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameGameGear"), localizationService.GetLocalizedString("ManufacturerNameSega"), "\uf129", true, new HashSet<string> { ".gg" }),
                new GameSystemVM(LibRetriX.GPGX.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameMegaDrive"), localizationService.GetLocalizedString("ManufacturerNameSega"), "\uf124", true, new HashSet<string> { ".mds", ".md", ".smd", ".gen" }),
                new GameSystemVM(LibRetriX.GPGX.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameMegaCD"), localizationService.GetLocalizedString("ManufacturerNameSega"), "\uf124", false, new HashSet<string> { ".bin", ".cue", ".iso", ".chd" }, CDImageExtensions),
                //new GameSystemVM(LibRetriX.BeetleSaturn.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameSaturn"), localizationService.GetLocalizedString("ManufacturerNameSega"), "\uf124", false, null, CDImageExtensions),
                new GameSystemVM(LibRetriX.BeetlePSX.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNamePlayStation"), localizationService.GetLocalizedString("ManufacturerNameSony"), "\uf128", false, null, CDImageExtensions),
                new GameSystemVM(LibRetriX.BeetlePCEFast.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNamePCEngine"), localizationService.GetLocalizedString("ManufacturerNameNEC"), "\uf124", true, new HashSet<string> { ".pce" }),
                new GameSystemVM(LibRetriX.BeetlePCEFast.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNamePCEngineCD"), localizationService.GetLocalizedString("ManufacturerNameNEC"), "\uf124", false, new HashSet<string> { ".cue", ".ccd", ".chd" }, CDImageExtensions),
                new GameSystemVM(LibRetriX.BeetlePCFX.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNamePCFX"), localizationService.GetLocalizedString("ManufacturerNameNEC"), "\uf124", false, null, CDImageExtensions),
                new GameSystemVM(LibRetriX.BeetleWswan.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameWonderSwan"), localizationService.GetLocalizedString("ManufacturerNameBandai"), "\uf129"),
                new GameSystemVM(LibRetriX.FBAlpha.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameNeoGeo"), localizationService.GetLocalizedString("ManufacturerNameSNK"), "\uf102", false),
                new GameSystemVM(LibRetriX.BeetleNGP.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameNeoGeoPocket"), localizationService.GetLocalizedString("ManufacturerNameSNK"), "\uf129"),
                new GameSystemVM(LibRetriX.FBAlpha.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameArcade"), localizationService.GetLocalizedString("ManufacturerNameFBAlpha"), "\uf102", true),
            };
        }

        public async Task<bool> StartGameAsync(GameSystemVM system, IFileInfo file, IDirectoryInfo rootFolder)
        {
            if (StartStopOperationInProgress)
            {
                return false;
            }

            StartStopOperationInProgress = true;

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
                if (CurrentCore != null)
                {
                    await Task.Run(() => CurrentCore.UnloadGame());
                }

                StreamProvider = provider;
                SaveStateService.SetGameId(virtualMainFilePath);
                CorePaused = false;
                CurrentCore = system.Core;

                loadSuccessful = await Task.Run(() =>
                {
                    try
                    {
                        return CurrentCore.LoadGame(virtualMainFilePath);
                    }
                    catch
                    {
                        return false;
                    }
                });

                if (!loadSuccessful)
                {
                    await StopGameAsyncInternal(true);
                    StartStopOperationInProgress = false;
                    return loadSuccessful;
                }
            }
            finally
            {
                CoreSemaphore.Release();
            }

            GameStarted?.Invoke(this);
            StartStopOperationInProgress = false;
            return loadSuccessful;
        }

        public async Task ResetGameAsync()
        {
            await CoreSemaphore.WaitAsync();
            try
            {
                if (CurrentCore != null)
                {
                    await Task.Run(() => CurrentCore.Reset());
                }
            }
            finally
            {
                CoreSemaphore.Release();
            }
        }

        public Task StopGameAsync()
        {
            return StopGameAsync(true);
        }

        public async Task StopGameAsync(bool performBackNavigation)
        {
            if (StartStopOperationInProgress)
            {
                return;
            }

            StartStopOperationInProgress = true;

            await CoreSemaphore.WaitAsync();
            try
            {
                await StopGameAsyncInternal(performBackNavigation);
            }
            finally
            {
                CoreSemaphore.Release();
            }

            GameStopped?.Invoke(this);
            StartStopOperationInProgress = false;
        }

        private async Task StopGameAsyncInternal(bool performBackNavigation)
        {
            if (CurrentCore != null)
            {
                await Task.Run(() => CurrentCore.UnloadGame());
                CurrentCore = null;
            }

            SaveStateService.SetGameId(null);
            StreamProvider = null;

            var initTasks = new Task[] { InputService.DeinitAsync(), AudioService.DeinitAsync(), VideoService.DeinitAsync() };
            await Task.WhenAll(initTasks);

            if (performBackNavigation && NavigationService.CurrentPageKey == GamePlayerPageKey)
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
            if (CurrentCore == null)
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
                    success = await Task.Run(() => CurrentCore.SaveState(stream));
                }
                finally
                {
                    CoreSemaphore.Release();
                }

                await stream.FlushAsync();
            }

            if (success)
            {
                var notificationBody = string.Format(Resources.Strings.StateSavedToSlotMessageBody, slotID);
                NotificationService.Show(Resources.Strings.StateSavedToSlotMessageTitle, notificationBody);
            }

            return success;
        }

        public async Task<bool> LoadGameStateAsync(uint slotID)
        {
            var success = false;
            if (CurrentCore == null)
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
                    success = await Task.Run(() => CurrentCore.LoadState(stream));
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
            CoreSemaphore.WaitAsync().Wait();
            try
            {
                if (!(CurrentCore == null || CorePaused || AudioService.ShouldDelayNextFrame))
                {
                    CurrentCore.RunFrame();
                }
            }
            catch (Exception e)
            {
                if (!StartStopOperationInProgress)
                {
                    StartStopOperationInProgress = true;
                    StopGameAsyncInternal(true).Wait();
                    StartStopOperationInProgress = false;
                }

                var task = PlatformService.RunOnUIThreadAsync(() => GameRuntimeExceptionOccurred?.Invoke(this, e));
            }
            finally
            {
                CoreSemaphore.Release();
            }
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
