using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Graphics.DirectX;

namespace LibretroRT.FrontendComponents.Win2DRenderer
{
    internal class RenderTargetManager : IDisposable
    {
        private const uint CoreRenderTargetMinSize = 1024;

        private static readonly IReadOnlyDictionary<PixelFormats, int> PixelFormatsSizeMapping = new Dictionary<PixelFormats, int>
        {
            { PixelFormats.FormatXRGB8888, 4 },
            { PixelFormats.FormatRGB565, 2 },
            { PixelFormats.Format0RGB1555, 2 },
        };

        private readonly object RenderTargetLock = new object();
        private CanvasBitmap RenderTarget = null;
        private byte[] RenderTargetBuffer = null;
        private Rect RenderTargetViewport = new Rect();

        private PixelFormats currentCorePixelFormat;
        public PixelFormats CurrentCorePixelFormat
        {
            get { return currentCorePixelFormat; }
            set { currentCorePixelFormat = value; RenderTargetPixelSize = PixelFormatsSizeMapping[value]; }
        }

        private int RenderTargetPixelSize = 0;

        public void Dispose()
        {
            RenderTarget.Dispose();
            RenderTarget = null;
        }

        public void Render(CanvasDrawingSession drawingSession, Size canvasSize)
        {
            var viewportWidth = RenderTargetViewport.Width;
            var viewportHeight = RenderTargetViewport.Height;

            if (RenderTarget == null || viewportWidth <= 0 || viewportHeight <= 0)
                return;

            lock (RenderTargetLock)
            {
                var destinationRect = ComputeBestFittingSize(canvasSize, (float)(viewportWidth / viewportHeight));
                drawingSession.DrawImage(RenderTarget, destinationRect, RenderTargetViewport);
            }
        }

        public void UpdateFromCoreOutput(byte[] frameBuffer, uint width, uint height, uint pitch)
        {
            if (frameBuffer == null || RenderTarget == null || RenderTargetPixelSize == 0)
                return;

            lock (RenderTargetLock)
            {
                var virtualWidth = pitch / RenderTargetPixelSize;
                RenderTargetViewport.Width = width;
                RenderTargetViewport.Height = height;

                switch(CurrentCorePixelFormat)
                {
                    case PixelFormats.FormatXRGB8888:
                        RenderTarget.SetPixelBytes(frameBuffer, 0, 0, (int)virtualWidth, (int)height);
                        break;
                    case PixelFormats.FormatRGB565:
                        ColorConverter.ConvertFrameBufferRGB565ToXRGB8888(frameBuffer, width, height, pitch, RenderTargetBuffer, RenderTarget.SizeInPixels.Width * 4);
                        RenderTarget.SetPixelBytes(RenderTargetBuffer, 0, 0, (int)virtualWidth, (int)height);
                        break;
                }
            }
        }

        public void UpdateRenderTargetSize(ICanvasResourceCreator resourceCreator, GameGeometry geometry)
        {
            if (RenderTarget != null)
            {
                var currentSize = RenderTarget.Size;
                if (currentSize.Width >= geometry.MaxWidth && currentSize.Height >= geometry.MaxHeight)
                {
                    return;
                }
            }

            lock (RenderTargetLock)
            {
                var size = Math.Max(Math.Max(geometry.MaxWidth, geometry.MaxHeight), CoreRenderTargetMinSize);
                size = ClosestGreaterPowerTwo(size);
                RenderTargetBuffer = new byte[size * size * 4];

                RenderTarget?.Dispose();
                RenderTarget = CanvasBitmap.CreateFromBytes(resourceCreator, RenderTargetBuffer, (int)size, (int)size, DirectXPixelFormat.B8G8R8A8UIntNormalized);
            }
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
