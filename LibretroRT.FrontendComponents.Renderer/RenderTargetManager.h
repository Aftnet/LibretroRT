#pragma once

#include "OpenGLES.h"

using namespace LibretroRT;

using namespace Concurrency;
using namespace Microsoft::Graphics::Canvas;
using namespace Microsoft::Graphics::Canvas::UI::Xaml;
using namespace Microsoft::WRL;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Graphics::DirectX::Direct3D11;

namespace LibretroRT_FrontendComponents_Renderer
{
	public ref class RenderTargetManager sealed
	{
	public:
		property GameGeometry^ Geometry
		{
			GameGeometry^ get() { return geometry; }
			void set(GameGeometry^ value) { geometry = value; }
		}

		property PixelFormats PixelFormat
		{
			PixelFormats get() { return pixelFormat; }
			void set(PixelFormats value) { pixelFormat = value; UpdateFormat(); }
		}

		RenderTargetManager(CanvasAnimatedControl^ canvas);
		virtual ~RenderTargetManager();

		void UpdateFromCoreOutput(const Array<byte>^ frameBuffer, unsigned int width, unsigned int height, unsigned int pitch);
		void Render(CanvasDrawingSession^ drawingSession, Size canvasSize);

	private:
		static const unsigned int RenderTargetMinSize = 1024;
		static const std::map<PixelFormats, DXGI_FORMAT> LibretroToDXGITextureFormatsMapping;

		GameGeometry^ geometry = nullptr;
		PixelFormats pixelFormat = PixelFormats::FormatUknown;

		CanvasAnimatedControl^ const Canvas;
		critical_section RenderTargetCriticalSection;

		ComPtr<ID3D11Device> Device;
		ComPtr<ID3D11Texture2D> D3DTexture;

		std::shared_ptr<OpenGLES> OpenGLESManager;
		EGLSurface OpenGLESSurface = EGL_NO_SURFACE;
		GLuint OpenGLESTexture = EGL_NO_TEXTURE;
		Rect RenderTargetViewport;

		void UpdateFormat();
		void CreateRenderTargets(ComPtr<ID3D11Device> device, unsigned int width, unsigned int height);
		void DestroyRenderTargets();
		static Rect ComputeBestFittingSize(Size viewportSize, float aspectRatio);
		static unsigned int ClosestGreaterPowerTwo(unsigned int value);
	};
}
