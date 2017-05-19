#pragma once

using namespace LibretroRT;
using namespace Platform;

struct retro_system_info;
struct retro_system_av_info;

namespace LibretroRTSupport
{
	private ref class CoreBase : public ICore
	{
	private:
		SystemTiming^ timing;
		GameGeometry^ geometry;
		String^ supportedExtensions;
		String^ version;
		String^ name;

	protected private:
		CoreBase();

	internal:
		void SetSystemInfo(retro_system_info& info);
		void SetAVInfo(retro_system_av_info & info);

		virtual bool EnvironmentHandler(unsigned cmd, void *data);

		void RaisePollInput();

	public:
		virtual ~CoreBase();

		virtual property SystemTiming^ Timing { SystemTiming^ get() { return ref new SystemTiming(timing); } }
		virtual property GameGeometry^ Geometry { GameGeometry^ get() { return ref new GameGeometry(geometry); } }
		virtual property String^ SupportedExtensions { String^ get() { return supportedExtensions; } }
		virtual property String^ Version { String^ get() { return version; } }
		virtual property String^ Name { String^ get() { return name; } }

		virtual event GetInputStateDelegate ^ GetInputState;
		virtual event PollInputDelegate ^ PollInput;
		virtual event RenderAudioFramesDelegate ^ RenderAudioFrames;
		virtual event RenderVideoFrameDelegate ^ RenderVideoFrame;

		virtual void LoadGame(Windows::Storage::Streams::IRandomAccessStream ^gameStream) = 0;
		virtual void UnloadGame() = 0;
		virtual void RunFrame() = 0;
		virtual void Reset() = 0;
	};
}


