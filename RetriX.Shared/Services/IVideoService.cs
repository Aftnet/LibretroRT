using LibRetriX;
using System;
using System.Threading.Tasks;

namespace RetriX.Shared.Services
{
    public interface IVideoService : IInitializable
    {
        void GeometryChanged(GameGeometry geometry);
        void PixelFormatChanged(PixelFormats format);
        void RotationChanged(Rotations rotation);
        void TimingsChanged(SystemTimings timings);
        void RenderVideoFrame(IntPtr data, uint width, uint height, ulong pitch);
    }
}
