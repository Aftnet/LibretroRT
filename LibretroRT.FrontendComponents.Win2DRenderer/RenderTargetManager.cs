using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Graphics.DirectX;

namespace LibretroRT.FrontendComponents.Win2DRenderer
{
    internal class RenderTargetManager : IDisposable
    {
        private const uint RenderTargetMinSize = 1024;
        private const DirectXPixelFormat RenderTargetPixelFormat = DirectXPixelFormat.B8G8R8A8UIntNormalized;
        private const int RenderTargetPixelSize = 4;

        private static readonly IReadOnlyDictionary<PixelFormats, int> PixelFormatsSizeMapping = new Dictionary<PixelFormats, int>
        {
            { PixelFormats.FormatXRGB8888, 4 },
            { PixelFormats.FormatRGB565, 2 },
            { PixelFormats.Format0RGB1555, 2 },
            { PixelFormats.FormatUknown, 0 },
        };

        private readonly object RenderTargetLock = new object();
        private CanvasBitmap RenderTarget = null;
        private byte[] RenderTargetBuffer = null;
        private Rect RenderTargetViewport = new Rect();
        //This may be different from viewport's width/haight.
        private float RenderTargetAspectRatio = 1.0f;

        private PixelFormats currentCorePixelFormat = PixelFormats.FormatUknown;
        public PixelFormats CurrentCorePixelFormat
        {
            get { return currentCorePixelFormat; }
            set { currentCorePixelFormat = value; CurrentCorePixelSize = PixelFormatsSizeMapping[currentCorePixelFormat]; }
        }

        private int CurrentCorePixelSize = 0;

        public void Dispose()
        {
            RenderTarget?.Dispose();
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
                var destinationRect = ComputeBestFittingSize(canvasSize, RenderTargetAspectRatio);
                drawingSession.DrawImage(RenderTarget, destinationRect, RenderTargetViewport);
            }
        }

        public void UpdateFromCoreOutput(byte[] frameBuffer, uint width, uint height, uint pitch)
        {
            if (frameBuffer == null || RenderTarget == null || CurrentCorePixelSize == 0)
                return;

            lock (RenderTargetLock)
            {
                var virtualWidth = pitch / CurrentCorePixelSize;
                RenderTargetViewport.Width = width;
                RenderTargetViewport.Height = height;

                switch(CurrentCorePixelFormat)
                {
                    case PixelFormats.FormatXRGB8888:
                        RenderTarget.SetPixelBytes(frameBuffer, 0, 0, (int)virtualWidth, (int)height);
                        break;
                    case PixelFormats.FormatRGB565:
                        ColorConverter.ConvertFrameBufferRGB565ToXRGB8888(frameBuffer, width, height, pitch, RenderTargetBuffer);
                        RenderTarget.SetPixelBytes(RenderTargetBuffer, 0, 0, (int)width, (int)height);
                        break;
                }
            }
        }

        public void UpdateRenderTargetSize(ICanvasResourceCreator resourceCreator, GameGeometry geometry)
        {
            RenderTargetAspectRatio = geometry.AspectRatio;
            if (RenderTargetAspectRatio < 0.1f)
            {
                RenderTargetAspectRatio = (float)(geometry.BaseWidth) / geometry.BaseHeight;
            }

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
                var size = Math.Max(Math.Max(geometry.MaxWidth, geometry.MaxHeight), RenderTargetMinSize);
                size = ClosestGreaterPowerTwo(size);
                RenderTargetBuffer = new byte[size * size * RenderTargetPixelSize];

                RenderTarget?.Dispose();
                RenderTarget = CanvasBitmap.CreateFromBytes(resourceCreator, RenderTargetBuffer, (int)size, (int)size, RenderTargetPixelFormat);
            }
        }

        private static Rect ComputeBestFittingSize(Size viewportSize, float aspectRatio)
        {
            Rect output;
            var candidateWidth = Math.Floor(viewportSize.Height * aspectRatio);
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
