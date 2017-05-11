#pragma once

#include "OpenGLES.h"

using namespace Windows::UI::Xaml::Controls;

ref class AngleSwapChainManager sealed
{
public:
	AngleSwapChainManager(SwapChainPanel^ swapChainPanel);
	virtual ~AngleSwapChainManager();

private:
	void OnPageLoaded(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
	void OnVisibilityChanged(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::VisibilityChangedEventArgs^ args);
	void CreateRenderSurface();
	void DestroyRenderSurface();
	void RecoverFromLostDevice();
	void StartRenderLoop();
	void StopRenderLoop();

	OpenGLES& mOpenGLES;

	SwapChainPanel^ mSwapChainPanel;
	EGLSurface mRenderSurface;     // This surface is associated with a swapChainPanel on the page

	Concurrency::critical_section mRenderSurfaceCriticalSection;
	Windows::Foundation::IAsyncAction^ mRenderLoopWorker;
};

