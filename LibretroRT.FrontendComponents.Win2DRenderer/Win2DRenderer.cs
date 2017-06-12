using LibretroRT.FrontendComponents.Common;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;

namespace LibretroRT.FrontendComponents.Win2DRenderer
{
    public sealed class Win2DRenderer : IRenderer, ICoreRunner, IDisposable
    {
        private readonly CoreEventCoordinator Coordinator;
        public bool CoreIsExecuting { get; private set; }

        private CanvasAnimatedControl RenderPanel;
        private bool RenderPanelInitialized = false;

        private readonly RenderTargetManager RenderTargetManager = new RenderTargetManager();

        public Win2DRenderer(CanvasAnimatedControl renderPanel, IAudioPlayer audioPlayer, IInputManager inputManager)
        {
            Coordinator = new CoreEventCoordinator
            {
                Renderer = this,
                AudioPlayer = audioPlayer,
                InputManager = inputManager
            };

            CoreIsExecuting = false;

            RenderPanel = renderPanel;
            RenderPanel.ClearColor = Color.FromArgb(0xff, 0, 0, 0);
            RenderPanel.Update -= RenderPanelUpdate;
            RenderPanel.Update += RenderPanelUpdate;
            RenderPanel.CreateResources -= RenderPanelCreateResources;
            RenderPanel.CreateResources += RenderPanelCreateResources;
            RenderPanel.Draw -= RenderPanelDraw;
            RenderPanel.Draw += RenderPanelDraw;
            RenderPanel.Unloaded -= RenderPanelUnloaded;
            RenderPanel.Unloaded += RenderPanelUnloaded;
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

        public async void LoadGame(ICore core, IStorageFile gameFile)
        {
            while (!RenderPanelInitialized)
            {
                //Ensure core doesn't try rendering before Win2D is ready.
                //Some games load faster than the Win2D canvas is initialized
                await Task.Delay(100);
            }

            lock (Coordinator)
            {
                Coordinator.Core?.UnloadGame();
                Coordinator.Core = core;
                core.LoadGame(gameFile);
                RenderTargetManager.CurrentCorePixelFormat = core.PixelFormat;
                CoreIsExecuting = true;
            }
        }

        public void UnloadGame()
        {
            lock (Coordinator)
            {
                CoreIsExecuting = false;
                Coordinator.AudioPlayer?.Stop();
                Coordinator.Core?.UnloadGame();
            }
        }

        public void ResetGame()
        {
            lock (Coordinator)
            {
                Coordinator.AudioPlayer?.Stop();
                Coordinator.Core?.Reset();
            }
        }

        public void PauseCoreExecution()
        {
            lock (Coordinator)
            {
                Coordinator.AudioPlayer?.Stop();
                CoreIsExecuting = false;
            }
        }

        public void ResumeCoreExecution()
        {
            lock (Coordinator)
            {
                CoreIsExecuting = true;
            }
        }

        private void RenderPanelUnloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            RenderPanel.Update -= RenderPanelUpdate;
            RenderPanel.Draw -= RenderPanelDraw;
            RenderPanel.Unloaded -= RenderPanelUnloaded;
            RenderPanel = null;
        }

        private void RenderPanelCreateResources(CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            RenderPanelInitialized = true;
        }

        private void RenderPanelUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            lock (Coordinator)
            {
                if (CoreIsExecuting && !Coordinator.AudioPlayerRequestsFrameDelay)
                {
                    Coordinator.Core?.RunFrame();
                }
            }
        }

        private void RenderPanelDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {

            RenderTargetManager.Render(args.DrawingSession, sender.Size);
        }

        public void RenderVideoFrame([ReadOnlyArray] byte[] frameBuffer, uint width, uint height, uint pitch)
        {
            RenderTargetManager.UpdateFromCoreOutput(frameBuffer, width, height, pitch);
        }

        public void GeometryChanged(GameGeometry geometry)
        {
            RenderTargetManager.UpdateRenderTargetSize(RenderPanel, geometry);
        }

        public void PixelFormatChanged(PixelFormats format)
        {
            RenderTargetManager.CurrentCorePixelFormat = format;
        }
    }
}
