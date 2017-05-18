#pragma once

using namespace Platform;

namespace GPGXRT
{
	public ref class GPGXCore sealed : public LibretroRT::ICore
	{
	private:
		String^ name;
		String^ version;
		String^ supportedExtensions;

	public:
		virtual property String^ Name
		{
			String^ get() { return name; }
		private:
			void set(String^ value) { name = value; }
		}

		virtual property String^ Version
		{
			String^ get() { return version; }
		private:
			void set(String^ value) { version = value; }
		}

		virtual property String^ SupportedExtensions
		{
			String^ get() { return supportedExtensions; }
		private:
			void set(String^ value) { supportedExtensions = value; }
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


