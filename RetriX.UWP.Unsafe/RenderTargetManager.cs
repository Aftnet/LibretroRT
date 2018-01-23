using LibRetriX;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Windows.Foundation;
using Windows.Graphics.DirectX;

namespace RetriX.UWP
{
    internal class RenderTargetManager : IDisposable
    {
        private const uint RenderTargetMinSize = 1024;
        private const DirectXPixelFormat RenderTargetPixelFormat = DirectXPixelFormat.B8G8R8A8UIntNormalized;
        private const int RenderTargetPixelSize = 4;

        private static readonly IReadOnlyDictionary<PixelFormats, int> PixelFormatsSizeMapping = new Dictionary<PixelFormats, int>
        {
            { PixelFormats.XRGB8888, 4 },
            { PixelFormats.RGB565, 2 },
            { PixelFormats.RGB0555, 2 },
            { PixelFormats.Unknown, 0 },
        };

        private readonly object RenderTargetLock = new object();
        private CanvasBitmap RenderTarget = null;
        private byte[] RenderTargetBuffer = null;
        private Rect RenderTargetViewport = new Rect();
        //This may be different from viewport's width/haight.
        private float RenderTargetAspectRatio = 1.0f;

        private PixelFormats currentCorePixelFormat = PixelFormats.Unknown;
        public PixelFormats CurrentCorePixelFormat
        {
            get { return currentCorePixelFormat; }
            set { currentCorePixelFormat = value; CurrentCorePixelSize = PixelFormatsSizeMapping[currentCorePixelFormat]; }
        }

        public Rotations CorrentRotation { get; set; }

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
            var aspectRatio = RenderTargetAspectRatio;
            if (RenderTarget == null || viewportWidth <= 0 || viewportHeight <= 0)
                return;

            var rotAngle = 0.0;
            switch (CorrentRotation)
            {
                case Rotations.CCW90:
                    rotAngle = -0.5 * Math.PI;
                    aspectRatio = 1.0f / aspectRatio;
                    break;
                case Rotations.CCW180:
                    rotAngle = -Math.PI;
                    break;
                case Rotations.CCW270:
                    rotAngle = -1.5 * Math.PI;
                    aspectRatio = 1.0f / aspectRatio;
                    break;
            }

            var destinationSize = ComputeBestFittingSize(canvasSize, aspectRatio);
            var scaleMatrix = Matrix3x2.CreateScale((float)destinationSize.Width, (float)destinationSize.Height);
            var rotMatrix = Matrix3x2.CreateRotation((float)rotAngle);
            var transMatrix = Matrix3x2.CreateTranslation((float)(0.5 * canvasSize.Width), (float)(0.5f * canvasSize.Height));
            var transformMatrix = rotMatrix * scaleMatrix * transMatrix;

            lock (RenderTargetLock)
            {
                drawingSession.Transform = transformMatrix;
                drawingSession.DrawImage(RenderTarget, new Rect(-0.5, -0.5, 1.0, 1.0), RenderTargetViewport);
                drawingSession.Transform = Matrix3x2.Identity;
            }
        }

        public void UpdateFromCoreOutput(Stream data, uint width, uint height, ulong pitch)
        {
            if (data == null || RenderTarget == null || CurrentCorePixelSize == 0)
                return;

            lock (RenderTargetLock)
            {
                var virtualWidth = (int)pitch / CurrentCorePixelSize;
                RenderTargetViewport.Width = width;
                RenderTargetViewport.Height = height;

                switch(CurrentCorePixelFormat)
                {
                    case PixelFormats.XRGB8888:
                        data.Read(RenderTargetBuffer, 0, (int)data.Length);
                        RenderTarget.SetPixelBytes(RenderTargetBuffer, 0, 0, virtualWidth, (int)height);
                        break;
                    case PixelFormats.RGB565:
                        ColorConverter.ConvertFrameBufferRGB565ToXRGB8888(data, width, height, pitch, RenderTargetBuffer);
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

        private static Size ComputeBestFittingSize(Size viewportSize, float aspectRatio)
        {
            var candidateWidth = Math.Floor(viewportSize.Height * aspectRatio);
            var size = new Size(candidateWidth, viewportSize.Height);
            if (viewportSize.Width < candidateWidth)
            {
                var height = viewportSize.Width / aspectRatio;
                size = new Size(viewportSize.Width, height);
            }

            return size;
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
