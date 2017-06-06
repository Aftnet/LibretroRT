using LibretroRT.FrontendComponents.Common;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;

namespace LibretroRT.FrontendComponents.Win2DRenderer
{
    public sealed class Win2DRenderer : IRenderer, ICoreRunner, IDisposable
    {
        private readonly CoreEventCoordinator Coordinator;
        private bool RunCore { get; set; }

        private readonly CanvasAnimatedControl RenderPanel;
        private readonly RenderTargetManager RenderTargetManager = new RenderTargetManager();

        public Win2DRenderer(CanvasAnimatedControl renderPanel, IAudioPlayer audioPlayer, IInputManager inputManager)
        {
            Coordinator = new CoreEventCoordinator
            {
                Renderer = this,
                AudioPlayer = audioPlayer,
                InputManager = inputManager
            };

            RunCore = false;

            RenderPanel = renderPanel;
            RenderPanel.Update += RenderPanelUpdate;
            RenderPanel.Draw += RenderPanelDraw;
            RenderPanel.Unloaded += RenderPanelUnloaded;
        }

        public void Dispose()
        {
            RenderTargetManager.Dispose();
        }

        public void LoadGame(ICore core, IStorageFile gameFile)
        {
            lock (Coordinator)
            {
                Coordinator.Core?.UnloadGame();
                Coordinator.Core = core;
                Coordinator.Core.LoadGame(gameFile);
                RunCore = true;
            }
        }

        public void UnloadGame()
        {
            lock (Coordinator)
            {
                RunCore = false;
                Coordinator.Core?.UnloadGame();
            }
        }

        public void ResetGame()
        {
            lock (Coordinator)
            {
                Coordinator.Core?.Reset();
            }
        }

        public void PauseCoreExecution()
        {
            lock (Coordinator)
            {
                RunCore = false;
            }
        }

        public void ResumeCoreExecution()
        {
            lock (Coordinator)
            {
                RunCore = true;
            }
        }

        private void RenderPanelUnloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            RenderPanel.Update -= RenderPanelUpdate;
            RenderPanel.Draw -= RenderPanelDraw;
            RenderPanel.Unloaded -= RenderPanelUnloaded;
        }

        private void RenderPanelUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            lock (Coordinator)
            {
                if (RunCore && !Coordinator.AudioPlayerRequestsFrameDelay)
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
            var core = Coordinator.Core;
            RenderTargetManager.UpdateFormat(RenderPanel, core.Geometry, core.PixelFormat);
        }

        public void PixelFormatChanged(PixelFormats format)
        {
            var core = Coordinator.Core;
            RenderTargetManager.UpdateFormat(RenderPanel, core.Geometry, core.PixelFormat);
        }
    }
}
