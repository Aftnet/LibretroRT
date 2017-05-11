#pragma once

#include <memory>

#include "OpenGLES.h"
#include "OpenGLESPage.g.h"
#include "AngleSwapChainManager.h"

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
