using System.Runtime.InteropServices.WindowsRuntime;

namespace LibretroRT.FrontendComponents.Common
{
    public interface IRenderer
    {
        void GeometryChanged(GameGeometry geometry);
        void PixelFormatChanged(PixelFormats format);
        void RotationChanged(Rotations rotation);
        void TimingChanged(SystemTiming timings);
        void RenderVideoFrame([ReadOnlyArray] byte[] frameBuffer, uint width, uint height, uint pitch);
    }
}
