using LibRetriX;
using System;

namespace RetriX.Shared.Services
{
    public delegate void RequestRunCoreFrameDelegate(IVideoService sender);

    public interface IVideoService : IInitializable
    {
        event RequestRunCoreFrameDelegate RequestRunCoreFrame;

        void GeometryChanged(GameGeometry geometry);
        void PixelFormatChanged(PixelFormats format);
        void RotationChanged(Rotations rotation);
        void TimingsChanged(SystemTimings timings);
        void RenderVideoFrame(IntPtr data, uint width, uint height, ulong pitch);
    }
}
