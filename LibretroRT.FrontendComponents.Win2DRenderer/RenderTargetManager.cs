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

        private readonly object RenderTargetLock = new object();
        private CanvasBitmap RenderTarget = null;
        private Rect RenderTargetViewport = new Rect();
        private int RenderTargetPixelSize = 0;

        public void Dispose()
        {
            RenderTarget.Dispose();
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
            if (frameBuffer == null || RenderTarget == null)
                return;

            lock (RenderTargetLock)
            {
                var virtualWidth = pitch / RenderTargetPixelSize;
                RenderTargetViewport.Width = width;
                RenderTargetViewport.Height = height;

                RenderTarget.SetPixelBytes(frameBuffer, 0, 0, (int)virtualWidth, (int)height);
            }
        }

        public void UpdateFormat(ICanvasResourceCreator resourceCreator, GameGeometry geometry, PixelFormats format)
        {
            var requestedFormat = PixelFormatsMapping[format];
            RenderTargetPixelSize = PixelFormatsSizeMapping[requestedFormat];

            if (RenderTarget != null)
            {
                var currentSize = RenderTarget.Size;
                if (currentSize.Width >= geometry.MaxWidth && currentSize.Height >= geometry.MaxHeight && RenderTarget.Format == requestedFormat)
                {
                    return;
                }
            }

            lock (RenderTargetLock)
            {
                var size = Math.Max(Math.Max(geometry.MaxWidth, geometry.MaxHeight), CoreRenderTargetMinSize);
                size = ClosestGreaterPowerTwo(size);
                var buffer = new byte[size * size * RenderTargetPixelSize];

                RenderTarget?.Dispose();
                RenderTarget = CanvasBitmap.CreateFromBytes(resourceCreator, buffer, (int)size, (int)size, requestedFormat);
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
