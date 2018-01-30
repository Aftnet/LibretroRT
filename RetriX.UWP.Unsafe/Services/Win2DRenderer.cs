using LibRetriX;
using Microsoft.Graphics.Canvas.UI.Xaml;
using RetriX.Shared.Services;
using RetriX.UWP.Components;
using System;
using System.Threading.Tasks;
using Windows.UI;

namespace RetriX.UWP
{
    public sealed class Win2DRenderer : IVideoService
    {
        public event RequestRunCoreFrameDelegate RequestRunCoreFrame;

        private CanvasAnimatedControl RenderPanel;
        private bool RenderPanelInitialized = false;

        private readonly RenderTargetManager RenderTargetManager = new RenderTargetManager();

        public Win2DRenderer(CanvasAnimatedControl renderPanel)
        {
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

        public Task InitAsync()
        {

        }

        public Task DeinitAsync()
        {
            RenderTargetManager.Dispose();
            return Task.CompletedTask;
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
            RequestRunCoreFrame?.Invoke(this);
        }

        private void RenderPanelDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            RenderTargetManager.Render(args.DrawingSession, sender.Size);
        }

        public void RenderVideoFrame(IntPtr data, uint width, uint height, ulong pitch)
        {
            RenderTargetManager.UpdateFromCoreOutput(RenderPanel.Device, data, width, height, pitch);
        }

        public void GeometryChanged(GameGeometry geometry)
        {
            RenderTargetManager.UpdateRenderTargetSize(RenderPanel.Device, geometry);
        }

        public void PixelFormatChanged(PixelFormats format)
        {
            RenderTargetManager.CurrentCorePixelFormat = format;
        }

        public void TimingsChanged(SystemTimings timings)
        {
            var targetTimeTicks = (long)(TimeSpan.TicksPerSecond / timings.FPS);
            RenderPanel.TargetElapsedTime = TimeSpan.FromTicks(targetTimeTicks);
        }

        public void RotationChanged(Rotations rotation)
        {
            RenderTargetManager.CorrentRotation = rotation;
        }
    }
}
