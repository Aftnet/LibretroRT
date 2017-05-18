#pragma once

#include "../LibretroRT/CoreHelper.h"

using namespace Platform;

namespace GPGXRT
{
	public ref class GPGXCore sealed : public LibretroRT::ICore
	{
	private:
		LibretroRT::CoreHelper^ helper;

	public:
		GPGXCore();
		virtual ~GPGXCore();

		virtual property String^ SupportedExtensions { String^ get() { return helper->SupportedExtensions; }}
		virtual property String^ Version { String^ get() { return helper->Version; }}
		virtual property String^ Name { String^ get() { return helper->Name; }}

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


