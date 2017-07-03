#pragma once

#include "RenderTargetManager.h"

using namespace LibretroRT;
using namespace LibretroRT::FrontendComponents::Common;

using namespace Concurrency;
using namespace Microsoft::Graphics::Canvas::UI;
using namespace Microsoft::Graphics::Canvas::UI::Xaml;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;

namespace LibretroRT_FrontendComponents_Win2DRendererNative
{
	public ref class Win2DRenderer sealed : IRenderer, ICoreRunner
	{
	private:
		CanvasAnimatedControl^ RenderPanel = nullptr;
		bool RenderPanelInitialized = false;

		RenderTargetManager RenderTargetManager;

		String^ gameID = nullptr;
		bool coreIsExecuting = false;
		CoreCoordinator^ Coordinator;
		critical_section CoordinatorCriticalSection;

		void RenderPanelUnloaded(Object^ sender, RoutedEventArgs^ e);
		void RenderPanelCreateResources(CanvasAnimatedControl^ sender, CanvasCreateResourcesEventArgs^ args);
		void RenderPanelUpdate(ICanvasAnimatedControl^ sender, CanvasAnimatedUpdateEventArgs^ args);
		void RenderPanelDraw(ICanvasAnimatedControl^ sender, CanvasAnimatedDrawEventArgs^ args);

	public:
		virtual event CoreRunExceptionOccurredDelegate^ CoreRunExceptionOccurred;

		virtual property String^ GameID
		{
			String^ get() { return gameID; }
		private:
			void set(String^ value) { gameID = value; }
		}

		virtual property bool CoreIsExecuting
		{
			bool get() { return coreIsExecuting; }
		private:
			void set(bool value) { coreIsExecuting = value; }
		}

		property unsigned int SerializationSize
		{
			virtual unsigned int get()
			{
				critical_section::scoped_lock lock(CoordinatorCriticalSection);

				auto core = Coordinator->Core;
				return core != nullptr ? core->SerializationSize : 0;
			}
		}

		Win2DRenderer(CanvasAnimatedControl^ renderPanel, IAudioPlayer^ audioPlayer, IInputManager^ inputManager);
		virtual ~Win2DRenderer();

		virtual IAsyncOperation<bool>^ LoadGameAsync(ICore^ core, String^ mainGameFilePath);
		virtual IAsyncAction^ UnloadGameAsync();
		virtual IAsyncAction^ ResetGameAsync();
		virtual IAsyncAction^ PauseCoreExecutionAsync();
		virtual IAsyncAction^ ResumeCoreExecutionAsync();
		virtual IAsyncOperation<bool>^ SaveGameStateAsync(WriteOnlyArray<byte>^ stateData);
		virtual IAsyncOperation<bool>^ LoadGameStateAsync(const Array<byte>^ stateData);
		virtual void RenderVideoFrame(const Array<byte>^ frameBuffer, unsigned int width, unsigned int height, unsigned int pitch);
		virtual void GeometryChanged(GameGeometry^ geometry);
		virtual void PixelFormatChanged(PixelFormats format);
	};
}