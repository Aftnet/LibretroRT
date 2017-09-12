using LibretroRT.FrontendComponents.Common;
using LibretroRT_FrontendComponents_Renderer;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace LibretroRT.FrontendComponents.Win2DCoreRunner
{
    public sealed class Win2DCoreRunner : ICoreRunner, IDisposable
    {
        public event CoreRunExceptionOccurredDelegate CoreRunExceptionOccurred;

        private readonly IAudioPlayer AudioPlayer;
        private readonly IInputManager InputManager;

        private TaskCompletionSource<bool> RequestedCoreOperationTCS;

        private CoreCoordinator Coordinator;

        public string GameID { get; private set; }
        public bool CoreIsExecuting { get; private set; }

        public uint SerializationSize
        {
            get
            {
                lock (Coordinator)
                {
                    var core = Coordinator.Core;
                    return core != null ? core.SerializationSize : 0;
                }
            }
        }

        private CanvasAnimatedControl RenderPanel;

        private Renderer RenderTargetManager;

        public Win2DCoreRunner(CanvasAnimatedControl renderPanel, IAudioPlayer audioPlayer, IInputManager inputManager)
        {
            RenderPanel = renderPanel;
            AudioPlayer = audioPlayer;
            InputManager = inputManager;

            CoreIsExecuting = false;

            RenderPanel.CreateResources -= RenderPanelCreateResources;
            RenderPanel.CreateResources += RenderPanelCreateResources;
        }

        public void Dispose()
        {
            lock (Coordinator)
            {
                Coordinator.Core?.UnloadGame();
                Coordinator.Dispose();
                RenderTargetManager.Dispose();
            }
        }

        public IAsyncOperation<bool> LoadGameAsync(ICore core, string mainGameFilePath)
        {
            if (RequestedCoreOperationTCS != null)
            {
                return Task.FromResult(false).AsAsyncOperation();
            }

            Func<bool> state = () =>
            {
                GameID = null;
                CoreIsExecuting = false;
                Coordinator.AudioPlayer?.Stop();

                if (Coordinator.Core != core)
                {
                    Coordinator.Core?.UnloadGame();
                    Coordinator.Core = core;
                }

                if (core.LoadGame(mainGameFilePath) == false)
                {
                    return false;
                }

                RenderTargetManager.InitializeVideoParameters(core);
                GameID = mainGameFilePath;
                CoreIsExecuting = true;
                return true;
            };

            RequestedCoreOperationTCS = new TaskCompletionSource<bool>(state);
            return RequestedCoreOperationTCS.Task.AsAsyncOperation();
        }

        public IAsyncAction UnloadGameAsync()
        {
            if (RequestedCoreOperationTCS != null)
            {
                return Task.FromResult(false).AsAsyncAction();
            }

            Func<bool> state = () =>
            {
                GameID = null;
                CoreIsExecuting = false;
                Coordinator.Core?.UnloadGame();
                Coordinator.AudioPlayer?.Stop();
                return true;
            };

            RequestedCoreOperationTCS = new TaskCompletionSource<bool>(state);
            return RequestedCoreOperationTCS.Task.AsAsyncAction();
        }

        public IAsyncAction ResetGameAsync()
        {
            if (RequestedCoreOperationTCS != null)
            {
                return Task.FromResult(false).AsAsyncAction();
            }

            Func<bool> state = () =>
            {
                Coordinator.AudioPlayer?.Stop();
                Coordinator.Core?.Reset();
                return true;
            };

            RequestedCoreOperationTCS = new TaskCompletionSource<bool>(state);
            return RequestedCoreOperationTCS.Task.AsAsyncAction();
        }

        public IAsyncAction PauseCoreExecutionAsync()
        {
            return Task.Run(() =>
            {
                lock (Coordinator)
                {
                    Coordinator.AudioPlayer?.Stop();
                    CoreIsExecuting = false;
                }
            }).AsAsyncAction();
        }

        public IAsyncAction ResumeCoreExecutionAsync()
        {
            return Task.Run(() =>
            {
                lock (Coordinator)
                {
                    CoreIsExecuting = true;
                }
            }).AsAsyncAction();
        }

        public IAsyncOperation<bool> SaveGameStateAsync([WriteOnlyArray] byte[] stateData)
        {
            if (RequestedCoreOperationTCS != null)
            {
                return Task.FromResult(false).AsAsyncOperation();
            }

            Func<bool> state = () =>
            {
                var core = Coordinator.Core;
                if (core == null)
                    return false;

                return core.Serialize(stateData);
            };

            RequestedCoreOperationTCS = new TaskCompletionSource<bool>(state);
            return RequestedCoreOperationTCS.Task.AsAsyncOperation();
        }

        public IAsyncOperation<bool> LoadGameStateAsync([ReadOnlyArray] byte[] stateData)
        {
            if (RequestedCoreOperationTCS != null)
            {
                return Task.FromResult(false).AsAsyncOperation();
            }

            Func<bool> state = () =>
            {
                var core = Coordinator.Core;
                if (core == null)
                    return false;

                return core.Unserialize(stateData);
            };

            RequestedCoreOperationTCS = new TaskCompletionSource<bool>(state);
            return RequestedCoreOperationTCS.Task.AsAsyncOperation();
        }

        private void RenderPanelUnloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            RenderPanel.CreateResources -= RenderPanelCreateResources;
            RenderPanel.Update -= RenderPanelUpdate;
            RenderPanel.Draw -= RenderTargetManager.CanvasDraw;
            RenderPanel.Unloaded -= RenderPanelUnloaded;
            RenderPanel = null;
        }

        private void RenderPanelCreateResources(CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            RenderTargetManager = new Renderer(RenderPanel);

            Coordinator = new CoreCoordinator
            {
                Renderer = RenderTargetManager,
                AudioPlayer = AudioPlayer,
                InputManager = InputManager
            };

            RenderPanel.ClearColor = Color.FromArgb(0xff, 0, 0, 0);
            RenderPanel.Update -= RenderPanelUpdate;
            RenderPanel.Update += RenderPanelUpdate;
            RenderPanel.Draw -= RenderTargetManager.CanvasDraw;
            RenderPanel.Draw += RenderTargetManager.CanvasDraw;
            RenderPanel.Unloaded -= RenderPanelUnloaded;
            RenderPanel.Unloaded += RenderPanelUnloaded;
        }

        private void RenderPanelUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            lock (Coordinator)
            {
                if (RequestedCoreOperationTCS != null)
                {
                    var requestedOperation = (Func<bool>)RequestedCoreOperationTCS.Task.AsyncState;
                    var result = requestedOperation.Invoke();
                    RequestedCoreOperationTCS.SetResult(result);
                    RequestedCoreOperationTCS = null;
                }

                if (CoreIsExecuting && !Coordinator.AudioPlayerRequestsFrameDelay)
                {
                    try
                    {
                        Coordinator.Core?.RunFrame();
                    }
                    catch (Exception e)
                    {
                        GameID = null;
                        CoreIsExecuting = false;
                        Coordinator.AudioPlayer?.Stop();
                        CoreRunExceptionOccurred(Coordinator.Core, e);
                    }
                }
            }
        }
    }
}
