#pragma once

using namespace LibretroRT;

using namespace Microsoft::Graphics::Canvas;
using namespace Microsoft::Graphics::Canvas::UI::Xaml;
using namespace Microsoft::WRL;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Graphics::DirectX::Direct3D11;

namespace LibretroRT_FrontendComponents_Win2DRendererNative
{
	class RenderTargetManager
	{
	public:
		RenderTargetManager(CanvasAnimatedControl^ canvas);
		~RenderTargetManager();

		void SetGameGeometry(GameGeometry^ geometry);
		void SetPixelFormat(PixelFormats pixelFormat);
		void UpdateFromCoreOutput(const Array<byte>^ frameBuffer, unsigned int width, unsigned int height, unsigned int pitch);
		void Render(CanvasDrawingSession^ drawingSession, Size canvasSize);

	private:
		static const unsigned int RenderTargetMinSize = 1024;

		CanvasAnimatedControl^ const Canvas;
		ComPtr<ID3D11Device> Device;
		ComPtr<ID3D11DeviceContext> DeviceContext;
		Rect RenderTargetViewport;
		float RenderTargetAspectRatio = 1.0f;

		static Rect ComputeBestFittingSize(Size viewportSize, float aspectRatio);
		static unsigned int ClosestGreaterPowerTwo(unsigned int value);
	};
}
