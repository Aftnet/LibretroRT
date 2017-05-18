#pragma once

#include "../LibretroRTSupport/CoreHelper.h"

using namespace Platform;
using namespace LibretroRTSupport;

namespace GPGXRT
{
	public ref class GPGXCore sealed : public LibretroRT::ICore
	{
	private:
		std::shared_ptr<CoreHelper> helper;

	public:
		virtual property String^ Name
		{
			String^ get() { return helper->GetName(); }
		}

		virtual property String^ Version
		{
			String^ get() { return helper->GetVersion(); }
		}

		virtual property String^ SupportedExtensions
		{
			String^ get() { return  helper->GetSupportedExtensions(); }
		}

		virtual property GameGeometry^ Geometry
		{
			GameGeometry^ get() { return  helper->GetGameGeometry(); }
		}

		virtual property SystemTiming^ Timing
		{
			SystemTiming^ get() { return  helper->GetSystemTiming(); }
		}

		GPGXCore();
		virtual ~GPGXCore();

		virtual void LoadGame(Windows::Storage::Streams::IRandomAccessStream ^gameStream);
		virtual void UnloadGame();
		virtual void RunFrame();
		virtual void Reset();

		virtual event LibretroRT::GetInputStateDelegate ^ GetInputState;
		virtual event LibretroRT::PollInputDelegate ^ PollInput;
		virtual event LibretroRT::RenderAudioFramesDelegate ^ RenderAudioFrames;
		virtual event LibretroRT::RenderVideoFrameDelegate ^ RenderVideoFrame;
	};
}


