#include "pch.h"
#include "Renderer.h"

using namespace LibretroRT_FrontendComponents_Renderer;

using namespace Windows::UI;

Renderer::Renderer(CanvasAnimatedControl^ renderPanel, IAudioPlayer^ audioPlayer, IInputManager^ inputManager):
	RenderPanel(renderPanel)
{
	Coordinator = ref new CoreCoordinator();
	Coordinator->Renderer = this;
	Coordinator->AudioPlayer = audioPlayer;
	Coordinator->InputManager = inputManager;

	Color clearColor;
	RenderPanel->ClearColor = clearColor;

	OnRenderPanelCreateResourcesToken = RenderPanel->CreateResources += ref new TypedEventHandler<CanvasAnimatedControl^, CanvasCreateResourcesEventArgs^>(this, &Renderer::OnRenderPanelCreateResources);
	OnRenderPanelUpdateToken = RenderPanel->Update += ref new TypedEventHandler<ICanvasAnimatedControl^, CanvasAnimatedUpdateEventArgs^>(this, &Renderer::OnRenderPanelUpdate);
	OnRenderPanelDrawToken = RenderPanel->Draw += ref new TypedEventHandler<ICanvasAnimatedControl^, CanvasAnimatedDrawEventArgs ^>(this, &Renderer::OnRenderPanelDraw);
	OnRenderPanelUnloadedToken = RenderPanel->Unloaded += ref new RoutedEventHandler(this, &Renderer::OnRenderPanelUnloaded);
}

Renderer::~Renderer()
{
	critical_section::scoped_lock lock(CoordinatorCriticalSection);

	RenderPanel->CreateResources -= OnRenderPanelCreateResourcesToken;
	RenderPanel->Update -= OnRenderPanelUpdateToken;
	RenderPanel->Draw -= OnRenderPanelDrawToken;
	RenderPanel->Unloaded -= OnRenderPanelUnloadedToken;

	auto core = Coordinator->Core;
	if (core) { core->UnloadGame(); }
}

IAsyncOperation<bool>^ Renderer::LoadGameAsync(ICore^ core, String^ mainGameFilePath)
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
		RenderManager->SetGameGeometry(core->Geometry);
		RenderManager->SetPixelFormat(core->PixelFormat);
		CoreIsExecuting = true;
		return true;
	});
}

IAsyncAction^ Renderer::UnloadGameAsync()
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

IAsyncAction^ Renderer::ResetGameAsync()
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

IAsyncAction^ Renderer::PauseCoreExecutionAsync()
{
	return create_async([this]()
	{
		critical_section::scoped_lock lock(CoordinatorCriticalSection);

		auto audioPlayer = Coordinator->AudioPlayer;
		if (audioPlayer) { audioPlayer->Stop(); }

		CoreIsExecuting = false;
	});
}

IAsyncAction^ Renderer::ResumeCoreExecutionAsync()
{
	return create_async([this]()
	{
		CoreIsExecuting = false;
	});
}

IAsyncOperation<bool>^ Renderer::SaveGameStateAsync(WriteOnlyArray<byte>^ stateData)
{
	return create_async([=]()
	{
		critical_section::scoped_lock lock(CoordinatorCriticalSection);

		auto core = Coordinator->Core;
		if (!core) { return false; }

		return core->Serialize(stateData);
	});
}

IAsyncOperation<bool>^ Renderer::LoadGameStateAsync(const Array<byte>^ stateData)
{
	return create_async([=]()
	{
		critical_section::scoped_lock lock(CoordinatorCriticalSection);

		auto core = Coordinator->Core;
		if (!core) { return false; }

		return core->Unserialize(stateData);
	});
}

void Renderer::RenderVideoFrame(const Array<byte>^ frameBuffer, unsigned int width, unsigned int height, unsigned int pitch)
{
	RenderManager->UpdateFromCoreOutput(frameBuffer, width, height, pitch);
}

void Renderer::GeometryChanged(GameGeometry^ geometry)
{
	RenderManager->SetGameGeometry(geometry);
}

void Renderer::PixelFormatChanged(PixelFormats format)
{
	RenderManager->SetPixelFormat(format);
}

void Renderer::OnRenderPanelCreateResources(CanvasAnimatedControl^ sender, CanvasCreateResourcesEventArgs^ args)
{
	RenderManager = std::make_unique<RenderTargetManager>(sender);
}

void Renderer::OnRenderPanelUpdate(ICanvasAnimatedControl^ sender, CanvasAnimatedUpdateEventArgs^ args)
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

void Renderer::OnRenderPanelDraw(ICanvasAnimatedControl^ sender, CanvasAnimatedDrawEventArgs^ args)
{
	RenderManager->Render(args->DrawingSession, sender->Size);
}

void Renderer::OnRenderPanelUnloaded(Object^ sender, RoutedEventArgs^ e)
{
	RenderManager.reset();

	RenderPanel->CreateResources -= OnRenderPanelCreateResourcesToken;
	RenderPanel->Update -= OnRenderPanelUpdateToken;
	RenderPanel->Draw -= OnRenderPanelDrawToken;
	RenderPanel->Unloaded -= OnRenderPanelUnloadedToken;
	RenderPanel = nullptr;
}