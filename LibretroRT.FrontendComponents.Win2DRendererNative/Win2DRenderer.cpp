#include "pch.h"
#include "Win2DRenderer.h"

using namespace LibretroRT_FrontendComponents_Win2DRendererNative;

using namespace Windows::UI;

Win2DRenderer::Win2DRenderer(CanvasAnimatedControl^ renderPanel, IAudioPlayer^ audioPlayer, IInputManager^ inputManager):
	RenderPanel(renderPanel)
{
	Coordinator = ref new CoreCoordinator();
	Coordinator->Renderer = this;
	Coordinator->AudioPlayer = audioPlayer;
	Coordinator->InputManager = inputManager;

	Color clearColor;
	RenderPanel->ClearColor = clearColor;

	OnRenderPanelCreateResourcesToken = RenderPanel->CreateResources += ref new TypedEventHandler<CanvasAnimatedControl^, CanvasCreateResourcesEventArgs^>(this, &Win2DRenderer::OnRenderPanelCreateResources);
	OnRenderPanelUpdateToken = RenderPanel->Update += ref new TypedEventHandler<ICanvasAnimatedControl^, CanvasAnimatedUpdateEventArgs^>(this, &Win2DRenderer::OnRenderPanelUpdate);
	OnRenderPanelDrawToken = RenderPanel->Draw += ref new TypedEventHandler<ICanvasAnimatedControl^, CanvasAnimatedDrawEventArgs ^>(this, &Win2DRenderer::OnRenderPanelDraw);
	OnRenderPanelUnloadedToken = RenderPanel->Unloaded += ref new RoutedEventHandler(this, &Win2DRenderer::OnRenderPanelUnloaded);
}

Win2DRenderer::~Win2DRenderer()
{
	critical_section::scoped_lock lock(CoordinatorCriticalSection);

	RenderPanel->CreateResources -= OnRenderPanelCreateResourcesToken;
	RenderPanel->Update -= OnRenderPanelUpdateToken;
	RenderPanel->Draw -= OnRenderPanelDrawToken;
	RenderPanel->Unloaded -= OnRenderPanelUnloadedToken;

	auto core = Coordinator->Core;
	if (core) { core->UnloadGame(); }
}

IAsyncOperation<bool>^ Win2DRenderer::LoadGameAsync(ICore^ core, String^ mainGameFilePath)
{
	return create_async([=]()-> bool
	{
		while (!RenderManager)
		{
			//Ensure core doesn't try rendering before Win2D is ready.
			//Some games load faster than the Win2D canvas is initialized
			std::this_thread::sleep_for(std::chrono::milliseconds(1000));
		}

		create_task(UnloadGameAsync()).wait();

		critical_section::scoped_lock lock(CoordinatorCriticalSection);

		Coordinator->Core = core;
		if (core->LoadGame(mainGameFilePath) == false)
		{
			return false;
		}

		GameID = mainGameFilePath;
		//RenderTargetManager->CurrentCorePixelFormat = core->PixelFormat;
		CoreIsExecuting = true;
		return true;
	});
}

IAsyncAction^ Win2DRenderer::UnloadGameAsync()
{
	return create_async([this]()
	{
		critical_section::scoped_lock lock(CoordinatorCriticalSection);

		GameID = nullptr;
		CoreIsExecuting = false;

		auto audioPlayer = Coordinator->AudioPlayer;
		if (audioPlayer) { audioPlayer->Stop(); }

		auto core = Coordinator->Core;
		if (core) { core->UnloadGame(); }
	});
}

IAsyncAction^ Win2DRenderer::ResetGameAsync()
{
	return create_async([this]()
	{
		critical_section::scoped_lock lock(CoordinatorCriticalSection);

		auto audioPlayer = Coordinator->AudioPlayer;
		if (audioPlayer) { audioPlayer->Stop(); }

		auto core = Coordinator->Core;
		if (core) { core->Reset(); }
	});
}

IAsyncAction^ Win2DRenderer::PauseCoreExecutionAsync()
{
	return create_async([this]()
	{
		critical_section::scoped_lock lock(CoordinatorCriticalSection);

		auto audioPlayer = Coordinator->AudioPlayer;
		if (audioPlayer) { audioPlayer->Stop(); }

		CoreIsExecuting = false;
	});
}

IAsyncAction^ Win2DRenderer::ResumeCoreExecutionAsync()
{
	return create_async([this]()
	{
		CoreIsExecuting = false;
	});
}

IAsyncOperation<bool>^ Win2DRenderer::SaveGameStateAsync(WriteOnlyArray<byte>^ stateData)
{
	return create_async([=]()
	{
		critical_section::scoped_lock lock(CoordinatorCriticalSection);

		auto core = Coordinator->Core;
		if (!core) { return false; }

		return core->Serialize(stateData);
	});
}

IAsyncOperation<bool>^ Win2DRenderer::LoadGameStateAsync(const Array<byte>^ stateData)
{
	return create_async([=]()
	{
		critical_section::scoped_lock lock(CoordinatorCriticalSection);

		auto core = Coordinator->Core;
		if (!core) { return false; }

		return core->Unserialize(stateData);
	});
}

void Win2DRenderer::RenderVideoFrame(const Array<byte>^ frameBuffer, unsigned int width, unsigned int height, unsigned int pitch)
{

}

void Win2DRenderer::GeometryChanged(GameGeometry^ geometry)
{

}

void Win2DRenderer::PixelFormatChanged(PixelFormats format)
{

}

void Win2DRenderer::OnRenderPanelCreateResources(CanvasAnimatedControl^ sender, CanvasCreateResourcesEventArgs^ args)
{
	RenderManager = std::make_unique<RenderTargetManager>(sender);
}

void Win2DRenderer::OnRenderPanelUpdate(ICanvasAnimatedControl^ sender, CanvasAnimatedUpdateEventArgs^ args)
{
	critical_section::scoped_lock lock(CoordinatorCriticalSection);

	if (CoreIsExecuting && !Coordinator->AudioPlayerRequestsFrameDelay)
	{
		auto core = Coordinator->Core;
		try
		{
			if (core) { core->RunFrame(); }
		}
		catch (Exception^ e)
		{
			GameID = nullptr;
			CoreIsExecuting = false;
			auto audioPlayer = Coordinator->AudioPlayer;
			if (audioPlayer) { audioPlayer->Stop(); }

			HResult hresult;
			hresult.Value = e->HResult;
			CoreRunExceptionOccurred(core, hresult);
		}
	}
}

void Win2DRenderer::OnRenderPanelDraw(ICanvasAnimatedControl^ sender, CanvasAnimatedDrawEventArgs^ args)
{

}

void Win2DRenderer::OnRenderPanelUnloaded(Object^ sender, RoutedEventArgs^ e)
{
	RenderManager.reset();

	RenderPanel->CreateResources -= OnRenderPanelCreateResourcesToken;
	RenderPanel->Update -= OnRenderPanelUpdateToken;
	RenderPanel->Draw -= OnRenderPanelDrawToken;
	RenderPanel->Unloaded -= OnRenderPanelUnloadedToken;
	RenderPanel = nullptr;
}