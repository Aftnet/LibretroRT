#pragma once

#include "OpenGLES.h"

namespace Angle
{
	public ref class OpenGLESSwapChain sealed : public Windows::UI::Xaml::Controls::SwapChainPanel
    {
    public:
        OpenGLESSwapChain();
        virtual ~OpenGLESSwapChain();

    private:
        void OnPageLoaded(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void OnVisibilityChanged(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::VisibilityChangedEventArgs^ args);
        void CreateRenderSurface();
        void DestroyRenderSurface();
        void RecoverFromLostDevice();
        void StartRenderLoop();
        void StopRenderLoop();

        static OpenGLES* mOpenGLES;

        EGLSurface mRenderSurface;     // This surface is associated with a swapChainPanel on the page
        Concurrency::critical_section mRenderSurfaceCriticalSection;
        Windows::Foundation::IAsyncAction^ mRenderLoopWorker;
    };
}
