#pragma once

using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml::Controls;

namespace LibretroRT_FrontendComponents_Renderer
{
	class OpenGLES
	{
	public:
		~OpenGLES();

		static std::shared_ptr<OpenGLES>& GetInstance();
		EGLSurface CreateSurface(EGLint width, EGLint height, EGLint format);
		EGLSurface CreateSurface(SwapChainPanel^ panel, const Size* renderSurfaceSize, const float* renderResolutionScale);
		EGLSurface CreateSurface(ComPtr<ID3D11Texture2D> d3dTexture);
		GLuint CreateTextureFromSurface(EGLSurface surface);
		void GetSurfaceDimensions(const EGLSurface surface, EGLint *width, EGLint *height);
		void DestroySurface(const EGLSurface surface);
		void DestroyTexture(const GLuint texture);
		void MakeCurrent(const EGLSurface surface);
		EGLBoolean SwapBuffers(const EGLSurface surface);
		void Reset();

	private:
		static std::mutex mMutex;

		OpenGLES();
		OpenGLES(const OpenGLES& rs);
		OpenGLES& operator = (const OpenGLES& rs);

		void Initialize();
		void Cleanup();

		EGLDisplay mEglDisplay;
		EGLContext mEglContext;
		EGLConfig  mEglConfig;
	};
}
