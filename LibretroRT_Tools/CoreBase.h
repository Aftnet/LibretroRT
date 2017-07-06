#pragma once

#include "../LibretroRT/libretro.h"

using namespace LibretroRT;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;

namespace LibretroRT_Tools
{
	typedef void (*LibretroGetSystemInfoPtr)(struct retro_system_info *info);
	typedef void (*LibretroGetSystemAVInfoPtr)(struct retro_system_av_info *info);
	typedef bool (*LibretroLoadGamePtr)(const struct retro_game_info *game);
	typedef void (*LibretroUnloadGamePtr)(void);
	typedef void (*LibretroRunPtr)(void);
	typedef void (*LibretroResetPtr)(void);
	typedef size_t (*LibretroSerializeSizePtr)(void);
	typedef bool (*LibretroSerializePtr)(void *data, size_t size);
	typedef bool (*LibretroUnserializePtr)(const void *data, size_t size);
	typedef void (*LibretroDeinitPtr)(void);

	private ref class CoreBase : public ICore
	{
	private:
		const LibretroGetSystemInfoPtr LibretroGetSystemInfo;
		const LibretroGetSystemAVInfoPtr LibretroGetSystemAVInfo;
		const LibretroLoadGamePtr LibretroLoadGame;
		const LibretroUnloadGamePtr LibretroUnloadGame;
		const LibretroRunPtr LibretroRun;
		const LibretroResetPtr LibretroReset;
		const LibretroSerializeSizePtr LibretroSerializeSize;
		const LibretroSerializePtr LibretroSerialize;
		const LibretroUnserializePtr LibretroUnserialize;
		const LibretroDeinitPtr LibretroDeinit;

		String^ name;
		String^ version;
		IStorageFolder^ systemFolder;
		std::string coreEnvironmentSystemFolderPath;
		const bool supportsSystemFolderVirtualization;
		IStorageFolder^ saveGameFolder;
		std::string coreEnvironmentSaveGameFolderPath;
		const bool supportsSaveGameFolderVirtualization;
		IVectorView<String^>^ supportedExtensions;
		IMap<String^, CoreOption^>^ const options;
		PixelFormats pixelFormat;
		GameGeometry^ geometry;
		SystemTiming^ timing;

		bool coreRequiresGameFilePath;
		bool gameLoaded;

	protected private:
		CoreBase(LibretroGetSystemInfoPtr libretroGetSystemInfo, LibretroGetSystemAVInfoPtr libretroGetSystemAVInfo,
			LibretroLoadGamePtr libretroLoadGame, LibretroUnloadGamePtr libretroUnloadGame, LibretroRunPtr libretroRun,
			LibretroResetPtr libretroReset, LibretroSerializeSizePtr libretroSerializeSize,
			LibretroSerializePtr libretroSerialize, LibretroUnserializePtr libretroUnserialize, LibretroDeinitPtr libretroDeinit,
			bool supportsSystemFolderVirtualization, bool supportsSaveGameFolderVirtualization);

		Vector<FileDependency^>^ fileDependencies;
		void ReadFileToMemory(String^ filePath, std::vector<unsigned char>& data);

	internal:
		virtual bool EnvironmentHandler(unsigned cmd, void *data);
		void SingleAudioFrameHandler(int16_t left, int16_t right);

		void RaisePollInput();
		int16_t RaiseGetInputState(unsigned port, unsigned device, unsigned index, unsigned keyId);
		size_t RaiseRenderAudioFrames(const int16_t* data, size_t frames);
		void RaiseRenderVideoFrame(const void* data, unsigned width, unsigned height, size_t pitch);

	public:
		virtual property String^ Name { String^ get() { return name; } }
		virtual property String^ Version { String^ get() { return version; } }
		virtual property IStorageFolder^ SystemFolder { IStorageFolder^ get() { return systemFolder; } }
		virtual property IStorageFolder^ SaveGameFolder { IStorageFolder^ get() { return saveGameFolder; } }
		virtual property IVectorView<String^>^ SupportedExtensions { IVectorView<String^>^ get() { return supportedExtensions; } }
		virtual property IMapView<String^, CoreOption^>^ Options { IMapView<String^, CoreOption^>^ get() { return options->GetView(); } };
		virtual property IVectorView<FileDependency^>^ FileDependencies { IVectorView<FileDependency^>^ get() { return fileDependencies->GetView(); } }

		virtual property PixelFormats PixelFormat
		{
			PixelFormats get() { return pixelFormat; }
		private:
			void set(PixelFormats value) { pixelFormat = value; if (PixelFormatChanged != nullptr) { PixelFormatChanged(pixelFormat); } }
		}

		virtual property GameGeometry^ Geometry
		{
			GameGeometry^ get() { return geometry; }
		private:
			void set(GameGeometry^ value) { geometry = value; if (GeometryChanged != nullptr) { GeometryChanged(geometry); } }
		}

		virtual property SystemTiming^ Timing
		{
			SystemTiming^ get() { return timing; }
		private:
			void set(SystemTiming^ value) { timing = value; if (TimingChanged != nullptr) { TimingChanged(timing); } }
		}

		virtual property unsigned int SerializationSize { unsigned int get() { return LibretroSerializeSize(); } }

		virtual property RenderVideoFrameDelegate ^ RenderVideoFrame;
		virtual property RenderAudioFramesDelegate ^ RenderAudioFrames;
		virtual property PollInputDelegate ^ PollInput;
		virtual property GetInputStateDelegate ^ GetInputState;
		virtual property GeometryChangedDelegate^ GeometryChanged;
		virtual property TimingChangedDelegate^ TimingChanged;
		virtual property PixelFormatChangedDelegate^ PixelFormatChanged;
		virtual property OpenFileStreamDelegate^ OpenFileStream;
		virtual property CloseFileStreamDelegate^ CloseFileStream;

		virtual ~CoreBase();

		virtual bool LoadGame(String^ mainGameFilePath);
		virtual void UnloadGame();
		virtual void RunFrame();
		virtual void Reset();

		virtual bool Serialize(WriteOnlyArray<uint8>^ stateData);
		virtual bool Unserialize(const Array<uint8>^ stateData);
	};
}


