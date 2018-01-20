using LibRetriX;
using System.IO;

namespace RetriX.UWP
{
    public interface IRenderer
    {
        void GeometryChanged(GameGeometry geometry);
        void PixelFormatChanged(PixelFormats format);
        void RotationChanged(Rotations rotation);
        void TimingsChanged(SystemTimings timings);
        void RenderVideoFrame(Stream data, uint width, uint height, ulong pitch);
    }
}
