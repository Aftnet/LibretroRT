#pragma once

using namespace LibretroRT;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;

struct retro_vfs_file_handle;

namespace LibretroRT_Shared
{
	private ref class CoreBase : public ICore
	{
	private:
		String^ name;
		String^ version;
		IStorageFolder^ systemFolder;
		std::string coreEnvironmentSystemFolderPath;
		const bool supportsSystemFolderVirtualization;
		IStorageFolder^ saveGameFolder;
		std::string coreEnvironmentSaveGameFolderPath;
		const bool supportsSaveGameFolderVirtualization;
		IVectorView<String^>^ supportedExtensions;
		bool nativeArchiveSupport;
		IMap<String^, CoreOption^>^ const options;
		PixelFormats pixelFormat;
		GameGeometry^ geometry;
		SystemTiming^ timing;

		bool coreRequiresGameFilePath;

		bool isInitialized;
		std::string gameFilePath;
		std::string lastResolvedEnvironmentVariable;

		void UnloadGameNoDeinit();

	protected private:
		CoreBase(bool supportsSystemFolderVirtualization, bool supportsSaveGameFolderVirtualization, bool nativeArchiveSupport);

		Vector<FileDependency^>^ fileDependencies;
		void ReadFileToMemory(String^ filePath, std::vector<unsigned char>& data);
		virtual void OverrideDefaultOptions(IMapView<String^, CoreOption^>^ options) { }

	internal:
		bool EnvironmentHandler(unsigned cmd, void *data);
		void SingleAudioFrameHandler(int16_t left, int16_t right);

		void RaisePollInput();
		int16_t RaiseGetInputState(unsigned port, unsigned device, unsigned index, unsigned keyId);
		size_t RaiseRenderAudioFrames(const int16_t* data, size_t frames);
		void RaiseRenderVideoFrame(const void* data, unsigned width, unsigned height, size_t pitch);

		const char* VFSGetPath(struct retro_vfs_file_handle* stream);
		struct retro_vfs_file_handle* VFSOpen(const char* path, unsigned mode, unsigned hints);
		int VFSClose(struct retro_vfs_file_handle* stream);
		int64_t VFSGetSize(struct retro_vfs_file_handle* stream);
		int64_t VFSGetPosition(struct retro_vfs_file_handle* stream);
		int64_t VFSSeek(struct retro_vfs_file_handle* stream, int64_t offset, int seek_position);
		int64_t VFSRead(struct retro_vfs_file_handle* stream, void* s, uint64_t len);
		int64_t VFSWrite(struct retro_vfs_file_handle* stream, const void* s, uint64_t len);
		int VFSFlush(struct retro_vfs_file_handle* stream);
		int VFSDelete(const char* path);
		int VFSRename(const char* old_path, const char* new_path);

	public:
		virtual property String^ Name { String^ get() { return name; } }
		virtual property String^ Version { String^ get() { return version; } }
		virtual property IStorageFolder^ SystemFolder { IStorageFolder^ get() { return systemFolder; } }
		virtual property IStorageFolder^ SaveGameFolder { IStorageFolder^ get() { return saveGameFolder; } }
		virtual property IVectorView<String^>^ SupportedExtensions { IVectorView<String^>^ get() { return supportedExtensions; } }
		virtual property bool NativeArchiveSupport { bool get() { return nativeArchiveSupport; } }
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

		virtual property unsigned int SerializationSize { unsigned int get(); }

		virtual property RenderVideoFrameDelegate^ RenderVideoFrame;
		virtual property RenderAudioFramesDelegate^ RenderAudioFrames;
		virtual property PollInputDelegate^ PollInput;
		virtual property GetInputStateDelegate^ GetInputState;
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
