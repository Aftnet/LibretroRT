﻿using LibretroRT;
using LibretroRT.FrontendComponents.Common;
using PCLStorage;
using RetriX.Shared.StreamProviders;
using RetriX.Shared.Services;
using RetriX.UWP.FileProviders;
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
using RetriX.Shared.ViewModels;
using Acr.UserDialogs;

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

        private readonly Lazy<FileImporterVM[]> dependencyImporters;
        public IReadOnlyList<FileImporterVM> DependencyImporters => dependencyImporters.Value;

        public string GameID => CoreRunner?.GameID;

        public event GameStartedDelegate GameStarted;
        public event GameRuntimeExceptionOccurredDelegate GameRuntimeExceptionOccurred;

        public EmulationService(IUserDialogs dialogsService, ILocalizationService localizationService, IPlatformService platformService, ICryptographyService cryptographyService)
        {
            LocalizationService = localizationService;
            PlatformService = platformService;

            RootFrame.Navigated += OnNavigated;

            systems = new GameSystemVM[]
            {
                new GameSystemVM(FCEUMMRT.FCEUMMCore.Instance, LocalizationService, "SystemNameNES", "ManufacturerNameNintendo", "\uf118", FCEUMMRT.FCEUMMCore.Instance.SupportedExtensions),
                new GameSystemVM(Snes9XRT.Snes9XCore.Instance, LocalizationService, "SystemNameSNES", "ManufacturerNameNintendo", "\uf119", Snes9XRT.Snes9XCore.Instance.SupportedExtensions),
                new GameSystemVM(GambatteRT.GambatteCore.Instance, LocalizationService, "SystemNameGameBoy", "ManufacturerNameNintendo", "\uf11b", GambatteRT.GambatteCore.Instance.SupportedExtensions),
                new GameSystemVM(VBAMRT.VBAMCore.Instance, LocalizationService, "SystemNameGameBoyAdvance", "ManufacturerNameNintendo", "\uf115", VBAMRT.VBAMCore.Instance.SupportedExtensions),
                new GameSystemVM(GPGXRT.GPGXCore.Instance, LocalizationService, "SystemNameSG1000", "ManufacturerNameSega", "\uf102", new string[]{ ".sg" }),
                new GameSystemVM(GPGXRT.GPGXCore.Instance, LocalizationService, "SystemNameMasterSystem", "ManufacturerNameSega", "\uf118", new string[]{ ".sms" }),
                new GameSystemVM(GPGXRT.GPGXCore.Instance, LocalizationService, "SystemNameGameGear", "ManufacturerNameSega", "\uf129", new string[]{ ".gg" }),
                new GameSystemVM(GPGXRT.GPGXCore.Instance, LocalizationService, "SystemNameMegaDrive", "ManufacturerNameSega", "\uf124", new string[]{ ".mds", ".md", ".smd", ".gen" }),
            };

            dependencyImporters = new Lazy<FileImporterVM[]>(() =>
            {
                return AvailableCores.Where(d => d.FileDependencies.Any()).SelectMany(d => d.FileDependencies.Select(e => new { core = d, deps = e }))
                    .Select(d => new FileImporterVM(dialogsService, localizationService, platformService, cryptographyService,
                    new WinRTFolder(d.core.SystemFolder), d.deps.Name, d.deps.Description, d.deps.MD5)).ToArray();
            });
        }

        public Task<bool> StartGameAsync(IFile file)
        {
            if (file == null)
            {
                throw new ArgumentException();
            }

            var fileExtension = Path.GetExtension(file.Path);
            foreach (var i in Systems)
            {
                if (i.SupportedExtensions.Contains(fileExtension))
                {
                    return StartGameAsync(i, file);
                }
            }

            throw new Exception("No compatible core found");
        }

        public async Task<bool> StartGameAsync(GameSystemVM system, IFile file)
        {
            if (file == null)
            {
                throw new ArgumentException();
            }

            if (CoreRunner == null)
            {
                RootFrame.Navigate(typeof(GamePlayerPage));
            }

            //Navigation should cause the player page to load, which in turn should initialize the core runner
            while (CoreRunner == null)
            {
                await Task.Delay(100);
            }

            return await StartGameAsync(CoreRunner, system, file);
        }

        private async Task<bool> StartGameAsync(ICoreRunner runner, GameSystemVM system, IFile file)
        {
            var mainGamePath = $"ROM\\{file.Name}";
            StreamProvider = new SingleFileStreamProvider(mainGamePath, file);
            var core = system.Core;
            core.GetFileStream = (d, e) =>
            {
                var accessMode = e == Windows.Storage.FileAccessMode.Read ? PCLStorage.FileAccess.Read : PCLStorage.FileAccess.ReadAndWrite;
                var output = StreamProvider.GetFileStreamAsync(d, accessMode).Result?.AsRandomAccessStream();
                return output;
            };

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
