#pragma once

#include "OpenGLES.h"
#include "IRenderer.h"

using namespace Windows::UI::Xaml::Controls;

namespace AngleSupport
{
	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class AngleSwapChainManager sealed
	{
	public:
		AngleSwapChainManager(SwapChainPanel^ swapChainPanel);
		virtual ~AngleSwapChainManager();
		void StartRenderer(IRenderer^ renderer);
		void StopRenderer();

	private:
		void OnPageLoaded(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void OnVisibilityChanged(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::VisibilityChangedEventArgs^ args);
		void CreateRenderSurface();
		void DestroyRenderSurface();
		void RecoverFromLostDevice();
		void StartRenderer();

		OpenGLES& mOpenGLES;

		SwapChainPanel^ mSwapChainPanel;
		EGLSurface mRenderSurface;     // This surface is associated with a swapChainPanel on the page

		IRenderer^ mRenderer;

		Concurrency::critical_section mRenderSurfaceCriticalSection;
		Windows::Foundation::IAsyncAction^ mRenderLoopWorker;
	};
}


