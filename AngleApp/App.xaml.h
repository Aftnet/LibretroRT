#pragma once

#include "app.g.h"
#include "OpenGLESPage.xaml.h"

namespace AngleApp
{
    ref class App sealed
    {
    public:
        App();
        virtual void OnLaunched(Windows::ApplicationModel::Activation::LaunchActivatedEventArgs^ e) override;

    private:
        OpenGLESPage^ mPage;
    };
}
