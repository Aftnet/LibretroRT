using LibRetriX;
using Microsoft.Graphics.Canvas.UI.Xaml;
using RetriX.Shared.Services;
using RetriX.UWP.Components;
using System;
using System.Threading.Tasks;
using Windows.UI;

namespace RetriX.UWP
{
    public sealed class VideoService : IVideoService
    {
        public event EventHandler RequestRunCoreFrame;

        private CanvasAnimatedControl renderPanel;
        public CanvasAnimatedControl RenderPanel
        {
            get => renderPanel;
            set
            {
                if (renderPanel == value)
                {
                    return;
                }

                if (renderPanel != null)
                {
                    renderPanel.Update -= RenderPanelUpdate;
                    renderPanel.Draw -= RenderPanelDraw;
                    renderPanel.Unloaded -= RenderPanelUnloaded;
                }

                renderPanel = value;
                if (renderPanel != null)
                {
                    RenderPanel.ClearColor = Color.FromArgb(0xff, 0, 0, 0);
                    renderPanel.Update += RenderPanelUpdate;
                    renderPanel.Draw += RenderPanelDraw;
                    renderPanel.Unloaded += RenderPanelUnloaded;
                }
            }
        }

        private readonly RenderTargetManager RenderTargetManager = new RenderTargetManager();

        private TaskCompletionSource<object> InitTCS;

        public Task InitAsync()
        {
            InitTCS = new TaskCompletionSource<object>();
            return InitTCS.Task;
        }

        public Task DeinitAsync()
        {
            RenderTargetManager.Dispose();
            return Task.CompletedTask;
        }

        private void RenderPanelUnloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            RenderPanel = null;
        }

        private void RenderPanelUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            if (InitTCS != null)
            {
                InitTCS.SetResult(null);
                InitTCS = null;
            }

            RequestRunCoreFrame?.Invoke(this, EventArgs.Empty);
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
