using Acr.UserDialogs;
using LibRetriX;
using MvvmCross.Core.Navigation;
using Plugin.FileSystem.Abstractions;
using Plugin.LocalNotifications.Abstractions;
using RetriX.Shared.StreamProviders;
using RetriX.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RetriX.Shared.Services
{
    public class EmulationService : IEmulationService
    {
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

        private IFileSystem FileSystem { get; }
        private IPlatformService PlatformService { get; }
        private ISaveStateService SaveStateService { get; }
        private ILocalNotifications NotificationService { get; }

        private IVideoService VideoService { get; }
        private IAudioService AudioService { get; }
        private IInputService InputService { get; }

        private bool CorePaused { get; set; } = false;
        private bool StartStopOperationInProgress { get; set; } = false;

        private SemaphoreSlim CoreSemaphore { get; } = new SemaphoreSlim(1, 1);

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

        public IReadOnlyList<GameSystemViewModel> Systems { get; }

        public event GameStartedDelegate GameStarted;
        public event GameStoppedDelegate GameStopped;
        public event GameRuntimeExceptionOccurredDelegate GameRuntimeExceptionOccurred;

        public EmulationService(IFileSystem fileSystem, IUserDialogs dialogsService,
            ILocalizationService localizationService, IPlatformService platformService, ISaveStateService saveStateService,
            ILocalNotifications notificationService, ICryptographyService cryptographyService,
            IVideoService videoService, IAudioService audioService, IInputService inputService)
        {
            FileSystem = fileSystem;
            PlatformService = platformService;
            SaveStateService = saveStateService;
            NotificationService = notificationService;

            VideoService = videoService;
            AudioService = audioService;
            InputService = inputService;

            VideoService.RequestRunCoreFrame += d => OnCoreRunFrameRequested();

            var CDImageExtensions = new HashSet<string> { ".bin", ".cue", ".iso", ".mds", ".mdf" };

            Systems = new GameSystemViewModel[]
            {
                new GameSystemViewModel(LibRetriX.FCEUMM.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameNES"), localizationService.GetLocalizedString("ManufacturerNameNintendo"), "\uf118"),
                new GameSystemViewModel(LibRetriX.Snes9X.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameSNES"), localizationService.GetLocalizedString("ManufacturerNameNintendo"), "\uf119"),
                //new GameSystemVM(LibRetriX.ParallelN64.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameNintendo64"), localizationService.GetLocalizedString("ManufacturerNameNintendo"), "\uf116"),
                new GameSystemViewModel(LibRetriX.Gambatte.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameGameBoy"), localizationService.GetLocalizedString("ManufacturerNameNintendo"), "\uf11b"),
                new GameSystemViewModel(LibRetriX.VBAM.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameGameBoyAdvance"), localizationService.GetLocalizedString("ManufacturerNameNintendo"), "\uf115"),
                new GameSystemViewModel(LibRetriX.MelonDS.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameDS"), localizationService.GetLocalizedString("ManufacturerNameNintendo"), "\uf117"),
                new GameSystemViewModel(LibRetriX.GPGX.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameSG1000"), localizationService.GetLocalizedString("ManufacturerNameSega"), "\uf102", true, new HashSet<string> { ".sg" }),
                new GameSystemViewModel(LibRetriX.GPGX.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameMasterSystem"), localizationService.GetLocalizedString("ManufacturerNameSega"), "\uf118", true, new HashSet<string> { ".sms" }),
                new GameSystemViewModel(LibRetriX.GPGX.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameGameGear"), localizationService.GetLocalizedString("ManufacturerNameSega"), "\uf129", true, new HashSet<string> { ".gg" }),
                new GameSystemViewModel(LibRetriX.GPGX.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameMegaDrive"), localizationService.GetLocalizedString("ManufacturerNameSega"), "\uf124", true, new HashSet<string> { ".mds", ".md", ".smd", ".gen" }),
                new GameSystemViewModel(LibRetriX.GPGX.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameMegaCD"), localizationService.GetLocalizedString("ManufacturerNameSega"), "\uf124", false, new HashSet<string> { ".bin", ".cue", ".iso", ".chd" }, CDImageExtensions),
                //new GameSystemVM(LibRetriX.BeetleSaturn.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameSaturn"), localizationService.GetLocalizedString("ManufacturerNameSega"), "\uf124", false, null, CDImageExtensions),
                new GameSystemViewModel(LibRetriX.BeetlePSX.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNamePlayStation"), localizationService.GetLocalizedString("ManufacturerNameSony"), "\uf128", false, null, CDImageExtensions),
                new GameSystemViewModel(LibRetriX.BeetlePCEFast.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNamePCEngine"), localizationService.GetLocalizedString("ManufacturerNameNEC"), "\uf124", true, new HashSet<string> { ".pce" }),
                new GameSystemViewModel(LibRetriX.BeetlePCEFast.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNamePCEngineCD"), localizationService.GetLocalizedString("ManufacturerNameNEC"), "\uf124", false, new HashSet<string> { ".cue", ".ccd", ".chd" }, CDImageExtensions),
                new GameSystemViewModel(LibRetriX.BeetlePCFX.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNamePCFX"), localizationService.GetLocalizedString("ManufacturerNameNEC"), "\uf124", false, null, CDImageExtensions),
                new GameSystemViewModel(LibRetriX.BeetleWswan.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameWonderSwan"), localizationService.GetLocalizedString("ManufacturerNameBandai"), "\uf129"),
                new GameSystemViewModel(LibRetriX.FBAlpha.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameNeoGeo"), localizationService.GetLocalizedString("ManufacturerNameSNK"), "\uf102", false),
                new GameSystemViewModel(LibRetriX.BeetleNGP.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameNeoGeoPocket"), localizationService.GetLocalizedString("ManufacturerNameSNK"), "\uf129"),
                new GameSystemViewModel(LibRetriX.FBAlpha.Core.Instance, FileSystem, localizationService.GetLocalizedString("SystemNameArcade"), localizationService.GetLocalizedString("ManufacturerNameFBAlpha"), "\uf102", true),
            };
        }

        public async Task<bool> StartGameAsync(ICore core, IStreamProvider streamProvider, string mainFilePath)
        {
            if (StartStopOperationInProgress)
            {
                return false;
            }

            StartStopOperationInProgress = true;

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

                StreamProvider = streamProvider;
                SaveStateService.SetGameId(mainFilePath);
                CorePaused = false;
                CurrentCore = core;

                loadSuccessful = await Task.Run(() =>
                {
                    try
                    {
                        return CurrentCore.LoadGame(mainFilePath);
                    }
                    catch
                    {
                        return false;
                    }
                });

                if (!loadSuccessful)
                {
                    await StopGameAsyncInternal();
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

        public async Task StopGameAsync()
        {
            if (StartStopOperationInProgress)
            {
                return;
            }

            StartStopOperationInProgress = true;

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
            StartStopOperationInProgress = false;
        }

        private async Task StopGameAsyncInternal()
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
                    StopGameAsyncInternal().Wait();
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
    }
}
