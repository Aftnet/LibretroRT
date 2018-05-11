using LibRetriX;
using System;
using System.IO;

namespace RetriX.Shared.Services
{
    public enum TextureFilterTypes { NearestNeighbor, Bilinear };

    public interface IVideoService : IInitializable
    {
        event EventHandler RequestRunCoreFrame;

        void GeometryChanged(GameGeometry geometry);
        void PixelFormatChanged(PixelFormats format);
        void RotationChanged(Rotations rotation);
        void TimingsChanged(SystemTimings timings);
        void RenderVideoFrame(Stream data, uint width, uint height, ulong pitch);
        void SetFilter(TextureFilterTypes filterType);
    }
}
