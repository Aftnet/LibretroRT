#pragma once

using namespace Microsoft::Graphics::Canvas::UI::Xaml;
using namespace Windows::Foundation;

namespace LibretroRT_FrontendComponents_Win2DRendererNative
{
	class RenderTargetManager
	{
	public:
		RenderTargetManager(CanvasAnimatedControl^ canvas);
		~RenderTargetManager();

	private:
		static const unsigned int RenderTargetMinSize = 1024;

		CanvasAnimatedControl^ const Canvas;
		Rect RenderTargetViewport;
		float RenderTargetAspectRatio = 1.0f;

		static Rect ComputeBestFittingSize(Size viewportSize, float aspectRatio);
		static unsigned int ClosestGreaterPowerTwo(unsigned int value);
	};
}
