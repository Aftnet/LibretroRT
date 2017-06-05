using LibretroRT.FrontendComponents.Common;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Graphics.DirectX;

namespace LibretroRT.FrontendComponents.Win2DRenderer
{
    public sealed class Win2DRenderer
    {
        private const uint CoreRenderTargetMinSize = 1024;

        private static readonly IReadOnlyDictionary<PixelFormats, DirectXPixelFormat> PixelFormatsMapping = new Dictionary<PixelFormats, DirectXPixelFormat>
        {
            { PixelFormats.FormatXRGB8888, DirectXPixelFormat.B8G8R8A8UIntNormalized },
            { PixelFormats.FormatRGB565, DirectXPixelFormat.B5G6R5UIntNormalized },
            { PixelFormats.Format0RGB1555, DirectXPixelFormat.B5G5R5A1UIntNormalized },
        };

        private static readonly IReadOnlyDictionary<DirectXPixelFormat, int> PixelFormatsSizeMapping = new Dictionary<DirectXPixelFormat, int>
        {
            { DirectXPixelFormat.B8G8R8A8UIntNormalized, 4 },
            { DirectXPixelFormat.B5G6R5UIntNormalized, 2 },
            { DirectXPixelFormat.B5G5R5A1UIntNormalized, 2 },
        };

        private readonly CanvasAnimatedControl RenderPanel;
        private CanvasBitmap CoreRenderTarget;
        private Rect CoreRenderTargetViewport = new Rect();

        private readonly object CoreLock = new object();

        public IAudioPlayer AudioPlayer { get; set; }
        private bool AudioPlayerWantsDelay { get { return AudioPlayer != null && AudioPlayer.ShouldDelayNextFrame; } }

        private ICore core = null;
        public ICore Core
        {
            get { return core; }
            set
            {
                if (core == value)
                {
                    return;
                }

                if (core != null)
                {
                    RunCore = false;
                    core.GeometryChanged -= CoreGameGeometryChanged;
                    core.PixelFormatChanged -= CorePixelFormatChanged;
                    core.RenderVideoFrame -= UpdateCoreRenderTarget;
                }

                core = value;
                if (core != null)
                {
                    core.GeometryChanged += CoreGameGeometryChanged;
                    core.PixelFormatChanged += CorePixelFormatChanged;
                    core.RenderVideoFrame += UpdateCoreRenderTarget;
                }
            }
        }

        private bool runCore = false;
        public bool RunCore
        {
            get { return runCore; }
            set
            {
                lock (CoreLock)
                {
                    runCore = value;
                }
            }
        }

        public Win2DRenderer(CanvasAnimatedControl renderPanel)
        {
            RenderPanel = renderPanel;
            RenderPanel.Update += RenderPanelUpdate;
            RenderPanel.Draw += RenderPanelDraw;
            RenderPanel.Unloaded += RenderPanelUnloaded;
        }

        private void RenderPanelUnloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            RenderPanel.Update -= RenderPanelUpdate;
            RenderPanel.Draw -= RenderPanelDraw;
            RenderPanel.Unloaded -= RenderPanelUnloaded;

            CoreRenderTarget?.Dispose();
            CoreRenderTarget = null;
        }

        private void RenderPanelUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            lock (CoreLock)
            {
                if (RunCore && !AudioPlayerWantsDelay)
                {
                    Core.RunFrame();
                }
            }
        }

        private void RenderPanelDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            var drawingSession = args.DrawingSession;

            var destinationRect = ComputeBestFittingSize(sender.Size, (float)(CoreRenderTargetViewport.Width / CoreRenderTargetViewport.Height));
            drawingSession.DrawImage(CoreRenderTarget, destinationRect, CoreRenderTargetViewport);
        }

        private void UpdateCoreRenderTarget(byte[] frameBuffer, uint width, uint height, uint pitch)
        {
            var virtualWidth = pitch / PixelFormatsSizeMapping[CoreRenderTarget.Format];
            CoreRenderTarget.SetPixelBytes(frameBuffer, 0, 0, (int)virtualWidth, (int)height);
            CoreRenderTargetViewport.Width = width;
            CoreRenderTargetViewport.Height = height;
        }

        private void CoreGameGeometryChanged(GameGeometry geometry)
        {
            UpdateFramebufferFormat(Core.Geometry, Core.PixelFormat);
        }

        private void CorePixelFormatChanged(PixelFormats format)
        {
            UpdateFramebufferFormat(Core.Geometry, Core.PixelFormat);
        }

        private void UpdateFramebufferFormat(GameGeometry geometry, PixelFormats format)
        {
            var requestedFormat = PixelFormatsMapping[format];
            if (CoreRenderTarget != null)
            {
                var currentSize = CoreRenderTarget.Size;
                if (currentSize.Width >= geometry.MaxWidth && currentSize.Height >= geometry.MaxHeight && CoreRenderTarget.Format == requestedFormat)
                {
                    return;
                }
            }

            var size = Math.Max(Math.Max(geometry.MaxWidth, geometry.MaxHeight), CoreRenderTargetMinSize);
            size = ClosestGreaterPowerTwo(size);

            var buffer = new byte[size * size * PixelFormatsSizeMapping[requestedFormat]];
            CoreRenderTarget?.Dispose();
            CoreRenderTarget = CanvasBitmap.CreateFromBytes(RenderPanel, buffer, (int)size, (int)size, PixelFormatsMapping[format]);
        }

        private static Rect ComputeBestFittingSize(Size viewportSize, float aspectRatio)
        {
            Rect output;
            var candidateWidth = viewportSize.Height * aspectRatio;
            if (viewportSize.Width >= candidateWidth)
            {
                var size = new Size(candidateWidth, viewportSize.Height);
                output = new Rect(new Point((viewportSize.Width - candidateWidth) / 2, 0), size);
            }
            else
            {
                var height = viewportSize.Width / aspectRatio;
                var size = new Size(viewportSize.Width, height);
                output = new Rect(new Point(0, (viewportSize.Height - height) / 2), size);
            }

            return output;
        }

        private static uint ClosestGreaterPowerTwo(uint value)
        {
            uint output = 1;
            while (output < value)
            {
                output *= 2;
            }

            return output;
        }
    }
}
