using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace LibretroRT.FrontendComponents.MonoGameRenderer
{
    internal class RenderTargetManager : IDisposable
    {
        private const uint CoreRenderTargetMinSize = 1024;

        private static readonly IReadOnlyDictionary<PixelFormats, SurfaceFormat> PixelFormatsMapping = new Dictionary<PixelFormats, SurfaceFormat>
        {
            { PixelFormats.FormatXRGB8888, SurfaceFormat.Color },
            { PixelFormats.FormatRGB565, SurfaceFormat.Bgr565 },
            { PixelFormats.Format0RGB1555, SurfaceFormat.Bgra5551 },
        };

        private static readonly IReadOnlyDictionary<SurfaceFormat, int> PixelFormatsSizeMapping = new Dictionary<SurfaceFormat, int>
        {
            { SurfaceFormat.Color, 4 },
            { SurfaceFormat.Bgr565, 2 },
            { SurfaceFormat.Bgra5551, 2 },
        };

        private readonly object RenderTargetLock = new object();
        private Texture2D RenderTarget = null;
        private Point RenderTargetViewportSize = new Point();
        private int RenderTargetPixelSize = 0;

        public void Dispose()
        {
            RenderTarget.Dispose();
        }

        public void Render(SpriteBatch spriteBatch, Point viewportSize)
        {
            if (RenderTarget == null || RenderTargetViewportSize.X <= 0 || RenderTargetViewportSize.Y <= 0)
                return;

            lock (RenderTargetLock)
            {
                var destinationRect = ComputeBestFittingPosition(viewportSize, RenderTargetViewportSize);
                spriteBatch.Draw(RenderTarget, destinationRect, new Rectangle(Point.Zero, RenderTargetViewportSize), Color.White);
            }
        }

        public void UpdateFromCoreOutput(byte[] frameBuffer, uint width, uint height, uint pitch)
        {
            if (frameBuffer == null || RenderTarget == null)
                return;

            lock (RenderTargetLock)
            {
                var virtualWidth = (int)(pitch / RenderTargetPixelSize);
                RenderTargetViewportSize.X = (int)width;
                RenderTargetViewportSize.Y = (int)height;

                var targetArea = new Rectangle(0, 0, virtualWidth, RenderTargetViewportSize.Y);
                RenderTarget.SetData(0, targetArea, frameBuffer, 0, frameBuffer.Length);
            }
        }

        public void UpdateFormat(GraphicsDevice resourceCreator, GameGeometry geometry, PixelFormats format)
        {
            var requestedFormat = PixelFormatsMapping[format];
            RenderTargetPixelSize = PixelFormatsSizeMapping[requestedFormat];

            if (RenderTarget != null)
            {
                var currentSize = RenderTarget.Bounds.Size;
                if (currentSize.X >= geometry.MaxWidth && currentSize.Y >= geometry.MaxHeight && RenderTarget.Format == requestedFormat)
                {
                    return;
                }
            }

            lock (RenderTargetLock)
            {
                var size = (int)Math.Max(Math.Max(geometry.MaxWidth, geometry.MaxHeight), CoreRenderTargetMinSize);
                size = ClosestGreaterPowerTwo(size);
                var buffer = new byte[size * size * RenderTargetPixelSize];

                RenderTarget?.Dispose();
                RenderTarget = new Texture2D(resourceCreator, size, size, false, requestedFormat);
            }
        }

        private static Rectangle ComputeBestFittingPosition(Point containerSize, Point contentSize)
        {
            var contentAspectRatio = contentSize.X / (float)contentSize.Y;

            Rectangle output;
            int candidateWidth = (int)(containerSize.Y * contentAspectRatio);
            if (containerSize.X >= candidateWidth)
            {
                var size = new Point(candidateWidth, containerSize.Y);
                output = new Rectangle(new Point((containerSize.X - candidateWidth) / 2, 0), size);
            }
            else
            {
                var height = (int)(containerSize.X / contentAspectRatio);
                var size = new Point(containerSize.X, height);
                output = new Rectangle(new Point(0, (containerSize.Y - height) / 2), size);
            }

            return output;
        }

        private static int ClosestGreaterPowerTwo(int value)
        {
            int output = 1;
            while (output < value)
            {
                output *= 2;
            }

            return output;
        }
    }
}
