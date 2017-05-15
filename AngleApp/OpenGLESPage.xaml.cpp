#include "pch.h"
#include "OpenGLESPage.xaml.h"
#include "SimpleRenderer.h"

using namespace AngleApp;
using namespace Platform;
using namespace Concurrency;
using namespace Windows::Foundation;

OpenGLESPage::OpenGLESPage()
{
    InitializeComponent();

    this->Loaded += ref new Windows::UI::Xaml::RoutedEventHandler(this, &OpenGLESPage::OnPageLoaded);

	mAngleSwapChainManager = ref new AngleSwapChainManager(swapChainPanel);
}

void OpenGLESPage::OnPageLoaded(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	auto renderer = ref new SimpleRenderer;
	mAngleSwapChainManager->StartRenderer(renderer);
}