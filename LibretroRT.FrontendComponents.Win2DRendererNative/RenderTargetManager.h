#pragma once

using namespace LibretroRT;

using namespace Microsoft::Graphics::Canvas;
using namespace Microsoft::Graphics::Canvas::UI::Xaml;
using namespace Platform;
using namespace Windows::Foundation;

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
		Rect RenderTargetViewport;
		float RenderTargetAspectRatio = 1.0f;

		static Rect ComputeBestFittingSize(Size viewportSize, float aspectRatio);
		static unsigned int ClosestGreaterPowerTwo(unsigned int value);
	};
}
