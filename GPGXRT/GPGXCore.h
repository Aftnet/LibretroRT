#pragma once

namespace GPGXRT
{
	public ref class GPGXCore sealed : public LibretroRT::ICore
	{
	public:
		GPGXCore();
		virtual ~GPGXCore();

	    virtual property Platform::String ^ SupportedExtensions;
		virtual property Platform::String ^ Version;
		virtual property Platform::String ^ Name;

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


