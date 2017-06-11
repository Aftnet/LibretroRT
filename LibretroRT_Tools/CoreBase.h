#pragma once

#include "../LibretroRT/libretro.h"

using namespace LibretroRT;
using namespace Platform;
using namespace Windows::Storage;

namespace LibretroRT_Tools
{
	private ref class CoreBase : public ICore
	{
	private:
		SystemTiming^ timing;
		GameGeometry^ geometry;
		String^ supportedExtensions;
		String^ version;
		String^ name;
		bool gameLoaded;

	protected private:
		Streams::IRandomAccessStream^ gameStream;
		PixelFormats pixelFormat;
		const std::string CoreSystemPath;
		const std::string CoreSaveGamePath;

		virtual bool LoadGameInternal(IStorageFile^ gameFile) = 0;
		virtual void UnloadGameInternal() = 0;

		CoreBase();
		void SetSystemInfo(retro_system_info& info);
		void SetAVInfo(retro_system_av_info & info);
		static retro_game_info GenerateGameInfo(String^ gamePath, unsigned long long gameSize);
		static retro_game_info GenerateGameInfo(const std::vector<unsigned char>& gameData);
		void ReadFileToMemory(std::vector<unsigned char>& data, IStorageFile^ file);

	internal:
		virtual bool EnvironmentHandler(unsigned cmd, void *data);
		void SingleAudioFrameHandler(int16_t left, int16_t right);
		size_t ReadGameFileHandler(void* buffer, size_t requested);
		void SeekGameFileHandler(unsigned long position);

		void RaisePollInput();
		int16_t RaiseGetInputState(unsigned port, unsigned device, unsigned index, unsigned keyId);
		size_t RaiseRenderAudioFrames(const int16_t* data, size_t frames);
		void RaiseRenderVideoFrame(const void* data, unsigned width, unsigned height, size_t pitch);

	public:
		virtual ~CoreBase();

		virtual property PixelFormats PixelFormat
		{
			PixelFormats get() { return pixelFormat; }
		private:
			void set(PixelFormats value) { pixelFormat = value; PixelFormatChanged(pixelFormat); }
		}
		virtual property SystemTiming^ Timing
		{
			SystemTiming^ get() { return timing; }
		private:
			void set(SystemTiming^ value) { timing = value; TimingChanged(timing); }
		}
		virtual property GameGeometry^ Geometry
		{
			GameGeometry^ get() { return geometry; }
		private:
			void set(GameGeometry^ value) { geometry = value; GeometryChanged(geometry); }
		}

		virtual property String^ SupportedExtensions { String^ get() { return supportedExtensions; } }
		virtual property String^ Version { String^ get() { return version; } }
		virtual property String^ Name { String^ get() { return name; } }
		virtual property unsigned int SerializationSize { unsigned int get() = 0; }

		virtual event GetInputStateDelegate ^ GetInputState;
		virtual event PollInputDelegate ^ PollInput;
		virtual event RenderAudioFramesDelegate ^ RenderAudioFrames;
		virtual event RenderVideoFrameDelegate ^ RenderVideoFrame;
		virtual event GeometryChangedDelegate^ GeometryChanged;
		virtual event TimingChangedDelegate^ TimingChanged;
		virtual event PixelFormatChangedDelegate^ PixelFormatChanged;

		virtual bool LoadGame(IStorageFile^ gameFile);
		virtual void UnloadGame();
		virtual void RunFrame() = 0;
		virtual void Reset() = 0;

		virtual bool Serialize(WriteOnlyArray<uint8>^ stateData) = 0;
		virtual bool Unserialize(const Array<uint8>^ stateData) = 0;
	};
}


