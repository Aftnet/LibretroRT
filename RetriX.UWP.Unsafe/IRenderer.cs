using LibRetriX;
using System;

namespace RetriX.UWP
{
    public interface IRenderer
    {
        void GeometryChanged(GameGeometry geometry);
        void PixelFormatChanged(PixelFormats format);
        void RotationChanged(Rotations rotation);
        void TimingsChanged(SystemTimings timings);
        void RenderVideoFrame(IntPtr data, uint width, uint height, ulong pitch);
    }
}
