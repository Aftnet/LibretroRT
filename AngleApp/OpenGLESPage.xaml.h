#pragma once

#include "OpenGLESPage.g.h"

using namespace AngleSupport;

namespace AngleApp
{
    public ref class OpenGLESPage sealed
    {
    public:
        OpenGLESPage();

    private:
		AngleSwapChainManager^ mAngleSwapChainManager;

		void OnPageLoaded(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    };
}
