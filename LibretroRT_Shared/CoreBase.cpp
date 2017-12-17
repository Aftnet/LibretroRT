#include "pch.h"
#include "CoreBase.h"
#include "Converter.h"
#include "NativeBuffer.h"
#include "StringConverter.h"
#include "libretro.h"

using namespace LibretroRT_Shared;
using namespace Windows::Storage;

struct retro_vfs_file_handle
{
	Platform::String^ const Path;
	IRandomAccessStream^ const Stream;

	retro_vfs_file_handle(Platform::String^ path, IRandomAccessStream^ stream):
		Path(path),
		Stream(stream)
	{
	}
};

CoreBase^ CoreBase::singletonInstance = nullptr;

void CoreBase::SingletonInstance::set(CoreBase^ value)
{
	singletonInstance = value;
	if (singletonInstance == nullptr)
	{
		return;
	}

	retro_set_environment([](unsigned cmd, void* data) { return singletonInstance->EnvironmentHandler(cmd, data); });
	retro_set_input_poll([]() { singletonInstance->RaisePollInput(); });
	retro_set_input_state([](unsigned port, unsigned device, unsigned index, unsigned keyId) { return singletonInstance->RaiseGetInputState(port, device, index, keyId); });
	retro_set_audio_sample([](int16_t left, int16_t right) { singletonInstance->SingleAudioFrameHandler(left, right); });
	retro_set_audio_sample_batch([](const int16_t* data, size_t numFrames) { return singletonInstance->RaiseRenderAudioFrames(data, numFrames); });
	retro_set_video_refresh([](const void *data, unsigned width, unsigned height, size_t pitch) { singletonInstance->RaiseRenderVideoFrame(data, width, height, pitch); });
}

CoreBase::CoreBase(bool supportsSystemFolderVirtualization, bool supportsSaveGameFolderVirtualization, bool nativeArchiveSupport) :
	supportsSystemFolderVirtualization(supportsSystemFolderVirtualization),
	supportsSaveGameFolderVirtualization(supportsSaveGameFolderVirtualization),
	nativeArchiveSupport(nativeArchiveSupport),
	options(ref new Map<String^, CoreOption^>()),
	pixelFormat(LibretroRT::PixelFormats::FormatRGB565),
	geometry(ref new GameGeometry),
	timing(ref new SystemTiming),
	coreRequiresGameFilePath(true),
	systemFolder(nullptr),
	saveGameFolder(nullptr),
	fileDependencies(ref new Vector<FileDependency^>),
	isInitialized(false)
{
	retro_system_info info;
	retro_get_system_info(&info);

	name = StringConverter::CPPToPlatformString(info.library_name);
	version = StringConverter::CPPToPlatformString(info.library_version);

	auto extensions = StringConverter::SplitString(info.valid_extensions, '|');
	auto extensionsVector = ref new Platform::Collections::Vector<String^>();
	for (auto i : extensions)
	{
		extensionsVector->Append(StringConverter::CPPToPlatformString("." + i));
	}
	supportedExtensions = extensionsVector->GetView();

	coreRequiresGameFilePath = info.need_fullpath;

	auto rootFolder = ApplicationData::Current->LocalFolder;

	auto systemFolderName = name->Concat(name, L"_System");
	systemFolder = concurrency::create_task(rootFolder->CreateFolderAsync(systemFolderName, CreationCollisionOption::OpenIfExists)).get();
	auto envPath = supportsSystemFolderVirtualization ? VFS::SystemPath : systemFolder->Path;
	coreEnvironmentSystemFolderPath.assign(StringConverter::PlatformToCPPString(envPath));

	auto saveGameFolderName = name->Concat(name, L"_Saves");
	saveGameFolder = concurrency::create_task(rootFolder->CreateFolderAsync(saveGameFolderName, CreationCollisionOption::OpenIfExists)).get();
	envPath = supportsSaveGameFolderVirtualization ? VFS::SavePath : saveGameFolder->Path;
	coreEnvironmentSaveGameFolderPath.assign(StringConverter::PlatformToCPPString(envPath));
}

CoreBase::~CoreBase()
{
}

unsigned int CoreBase::SerializationSize::get()
{
	return retro_serialize_size();
}

bool CoreBase::EnvironmentHandler(unsigned cmd, void *data)
{
	switch (cmd)
	{
	case RETRO_ENVIRONMENT_SET_VARIABLES:
	{
		auto dataPtr = reinterpret_cast<retro_variable*>(data);
		while (dataPtr->key)
		{
			auto option = Converter::RetroVariableToCoreOptionDescription(dataPtr->value);
			options->Insert(StringConverter::CPPToPlatformString(dataPtr->key), option);
			dataPtr++;
		}

		OverrideDefaultOptions(Options);
		return true;
	}
	case RETRO_ENVIRONMENT_GET_VARIABLE:
	{
		auto dataPtr = reinterpret_cast<retro_variable*>(data);
		dataPtr->value = nullptr;

		auto key = StringConverter::CPPToPlatformString(dataPtr->key);
		if (options->HasKey(key))
		{
			auto option = options->Lookup(key);
			auto selectedValue = option->Values->GetAt(option->SelectedValueIx);
			lastResolvedEnvironmentVariable = StringConverter::PlatformToCPPString(selectedValue);
			dataPtr->value = lastResolvedEnvironmentVariable.c_str();
			return true;
		}
		return false;
	}
	case RETRO_ENVIRONMENT_GET_OVERSCAN:
	{
		auto dataPtr = reinterpret_cast<bool*>(data);
		*dataPtr = false;
		return true;
	}
	case RETRO_ENVIRONMENT_GET_CAN_DUPE:
	{
		auto dataPtr = reinterpret_cast<bool*>(data);
		*dataPtr = true;
		return true;
	}
	case RETRO_ENVIRONMENT_GET_SYSTEM_DIRECTORY:
	{
		auto dataPtr = reinterpret_cast<const char**>(data);
		*dataPtr = coreEnvironmentSystemFolderPath.c_str();
		return true;
	}
	case RETRO_ENVIRONMENT_GET_SAVE_DIRECTORY:
	{
		auto dataPtr = reinterpret_cast<const char**>(data);
		*dataPtr = coreEnvironmentSaveGameFolderPath.c_str();
		return true;
	}
	case RETRO_ENVIRONMENT_GET_LOG_INTERFACE:
	{
		auto dataPtr = reinterpret_cast<retro_log_callback*>(data);
		dataPtr->log = [](enum retro_log_level level, const char *fmt, ...)
		{
#ifndef NDEBUG
			const int bufLen = 1024;
			static char logBuffer[bufLen];

			va_list args;
			va_start(args, fmt);
			vsnprintf_s(logBuffer, bufLen, fmt, args);
			va_end(args);

			auto debugMsg = StringConverter::CPPToPlatformString(logBuffer);
			OutputDebugString(debugMsg->Data());
#endif // DEBUG
		};
		return true;
	}
	case RETRO_ENVIRONMENT_SET_PIXEL_FORMAT:
	{
		auto dataPtr = reinterpret_cast<enum retro_pixel_format*>(data);
		PixelFormat = Converter::ConvertToPixelFormat(*dataPtr);
		return true;
	}
	case RETRO_ENVIRONMENT_SET_GEOMETRY:
	{
		auto dataPtr = reinterpret_cast<retro_game_geometry*>(data);
		Geometry = Converter::CToRTGameGeometry(*dataPtr);
		return true;
	}
	case RETRO_ENVIRONMENT_SET_SYSTEM_AV_INFO:
	{
		auto dataPtr = reinterpret_cast<retro_system_av_info*>(data);
		Geometry = Converter::CToRTGameGeometry(dataPtr->geometry);
		Timing = Converter::CToRTSystemTiming(dataPtr->timing);
		return true;
	}

	case RETRO_ENVIRONMENT_GET_VFS_INTERFACE:
	{
		const uint32_t SupportedVFSVersion = 1;
		static struct retro_vfs_interface vfsInterface =
		{
			[](struct retro_vfs_file_handle* stream) { return SingletonInstance->VFSGetPath(stream); },
			[](const char* path, unsigned mode, unsigned hints) { return SingletonInstance->VFSOpen(path, mode, hints); },
			[](struct retro_vfs_file_handle* stream) { return SingletonInstance->VFSClose(stream); },
			[](struct retro_vfs_file_handle* stream) { return SingletonInstance->VFSGetSize(stream); },
			[](struct retro_vfs_file_handle* stream) { return SingletonInstance->VFSGetPosition(stream); },
			[](struct retro_vfs_file_handle* stream, int64_t offset, int seek_position) { return SingletonInstance->VFSSeek(stream, offset, seek_position); },
			[](struct retro_vfs_file_handle* stream, void* s, uint64_t len) { return SingletonInstance->VFSRead(stream, s, len); },
			[](struct retro_vfs_file_handle* stream, const void* s, uint64_t len) { return SingletonInstance->VFSWrite(stream, s, len); },
			[](struct retro_vfs_file_handle* stream) { return SingletonInstance->VFSFlush(stream); },
			[](const char* path) { return SingletonInstance->VFSDelete(path); },
			[](const char* old_path, const char* new_path) { return SingletonInstance->VFSRename(old_path, new_path); },
		};

		auto dataPtr = reinterpret_cast<retro_vfs_interface_info*>(data);
		if (dataPtr->required_interface_version <= SupportedVFSVersion)
		{
			dataPtr->required_interface_version = SupportedVFSVersion;
			dataPtr->iface = &vfsInterface;
		}

		return true;
	}

	/*case RETRO_ENVIRONMENT_SET_HW_RENDER:
	{
	auto dataPtr = reinterpret_cast<retro_hw_render_callback*>(data);
	dataPtr->context_type = retro_hw_context_type::RETRO_HW_CONTEXT_OPENGLES2;
	dataPtr->context_reset = nullptr; //need
	dataPtr->get_current_framebuffer = nullptr;
	dataPtr->get_proc_address = nullptr; //need
	dataPtr->depth = false;
	dataPtr->stencil = false;
	dataPtr->bottom_left_origin = false; //need
	dataPtr->version_major = 2; //need
	dataPtr->version_minor = 0; //need
	dataPtr->cache_context = true; //need
	dataPtr->context_destroy = nullptr; //need
	dataPtr->debug_context = false; //need
	return true;
	}*/
	}
	return false;
}

void CoreBase::SingleAudioFrameHandler(int16_t left, int16_t right)
{
	int16_t data[2];
	data[0] = left;
	data[1] = right;
	RaiseRenderAudioFrames(data, 1);
}

void CoreBase::RaisePollInput()
{
	if (PollInput == nullptr)
		return;

	PollInput();
}

int16_t CoreBase::RaiseGetInputState(unsigned port, unsigned device, unsigned index, unsigned keyId)
{
	if (GetInputState == nullptr)
		return 0;

	auto key = Converter::ConvertToInputType(device, index, keyId);
	return GetInputState(port, key);
}

size_t CoreBase::RaiseRenderAudioFrames(const int16_t* data, size_t frames)
{
	if (RenderAudioFrames == nullptr)
		return 0;

	auto dataPtr = const_cast<int16_t*>(data);
	auto dataArray = Platform::ArrayReference<int16_t>(dataPtr, frames * 2);
	RenderAudioFrames(dataArray);
	return 0;
}

void CoreBase::RaiseRenderVideoFrame(const void* data, unsigned width, unsigned height, size_t pitch)
{
	if (RenderVideoFrame == nullptr)
		return;

	//Duped frame
	if (data == nullptr)
	{
		RenderVideoFrame(ref new Platform::Array<uint8>(0), width, height, pitch);
		return;
	}

	//Hardware rendering
	if (data == RETRO_HW_FRAME_BUFFER_VALID)
	{
		RenderVideoFrame(ref new Platform::Array<uint8>(0), width, height, pitch);
		return;
	}

	auto dataPtr = reinterpret_cast<uint8*>(const_cast<void*>(data));
	//See retro_video_refresh_t for why buffer size is computed that way
	auto dataArray = Platform::ArrayReference<uint8>(dataPtr, height * pitch);
	RenderVideoFrame(dataArray, width, height, pitch);
}

const char* CoreBase::VFSGetPath(struct retro_vfs_file_handle *stream)
{
	return StringConverter::PlatformToCPPString(stream->Path).data();
}

struct retro_vfs_file_handle* CoreBase::VFSOpen(const char* path, unsigned mode, unsigned hints)
{
	auto pathString = StringConverter::CPPToPlatformString(path);
	auto fileAcces = FileAccessMode::Read;
	if ((mode & RETRO_VFS_FILE_ACCESS_WRITE) != 0)
	{
		fileAcces = FileAccessMode::ReadWrite;
	}

	auto stream = OpenFileStream(pathString, fileAcces);
	if (stream == nullptr)
	{
		return nullptr;
	}

	auto output = new retro_vfs_file_handle(pathString, stream);
	return output;
}

int CoreBase::VFSClose(struct retro_vfs_file_handle* stream)
{
	VFSFlush(stream);
	CloseFileStream(stream->Stream);
	delete stream;
	return 0;
}

int64_t CoreBase::VFSGetSize(struct retro_vfs_file_handle* stream)
{
	return stream->Stream->Size;
}

int64_t CoreBase::VFSGetPosition(struct retro_vfs_file_handle* stream)
{
	return stream->Stream->Position;
}

int64_t CoreBase::VFSSeek(struct retro_vfs_file_handle* stream, int64_t offset, int seek_position)
{
	switch (seek_position)
	{
	case RETRO_VFS_SEEK_POSITION_CURRENT:
		offset += VFSGetPosition(stream);
	case RETRO_VFS_SEEK_POSITION_END:
		offset = VFSGetSize(stream) - offset;
	}

	stream->Stream->Seek(offset);
	return 0;
}

int64_t CoreBase::VFSRead(struct retro_vfs_file_handle* stream, void* s, uint64_t len)
{
	auto winStream = stream->Stream;
	size_t remaining = winStream->Size - winStream->Position;
	auto output = min(len, remaining);

	auto buffer = LibretroRT_Shared::CreateNativeBuffer(s, len);
	concurrency::create_task(winStream->ReadAsync(buffer, output, InputStreamOptions::None)).get();

	return output;
}

int64_t CoreBase::VFSWrite(struct retro_vfs_file_handle* stream, const void* s, uint64_t len)
{
	auto dataArray = Platform::ArrayReference<unsigned char>((unsigned char*)s, len);
	auto writer = ref new DataWriter(stream->Stream);

	writer->WriteBytes(dataArray);
	concurrency::create_task(stream->Stream->FlushAsync()).get();
	writer->DetachStream();

	return len;
}

int CoreBase::VFSFlush(struct retro_vfs_file_handle* stream)
{
	return 0;
}


int CoreBase::VFSDelete(const char *path)
{
	return -1;
}

int CoreBase::VFSRename(const char *old_path, const char *new_path)
{
	return -1;
}

bool CoreBase::LoadGame(String^ mainGameFilePath)
{
	if (!isInitialized)
	{
		retro_init();
		isInitialized = true;
	}

	if (!gameFilePath.empty())
	{
		UnloadGameNoDeinit();
	}

	try
	{
		gameFilePath = StringConverter::PlatformToCPPString(mainGameFilePath);
		retro_game_info gameInfo;
		gameInfo.data = nullptr;
		gameInfo.path = gameFilePath.c_str();
		gameInfo.size = 0;
		gameInfo.meta = nullptr;

		std::vector<unsigned char> gameData;
		if (!coreRequiresGameFilePath)
		{
			auto stream = VFSOpen(gameFilePath.data(), RETRO_VFS_FILE_ACCESS_READ, RETRO_VFS_FILE_ACCESS_HINT_NONE);
			auto size = VFSGetSize(stream);
			gameData.resize(size);
			auto readBytes = VFSRead(stream, gameData.data(), size);
			VFSClose(stream);

			gameInfo.data = gameData.data();
			gameInfo.size = size;
		}

		auto loadSuccessful = retro_load_game(&gameInfo);
		if (loadSuccessful)
		{
			retro_system_av_info info;
			retro_get_system_av_info(&info);

			Geometry = Converter::CToRTGameGeometry(info.geometry);
			Timing = Converter::CToRTSystemTiming(info.timing);

			retro_set_controller_port_device(0, RETRO_DEVICE_ANALOG);
		}
		else
		{
			gameFilePath.clear();
		}
	}
	catch (const std::exception& e)
	{
		UnloadGameNoDeinit();
	}

	return !gameFilePath.empty();
}

void CoreBase::UnloadGameNoDeinit()
{
	if (gameFilePath.empty())
	{
		return;
	}

	try
	{
		retro_unload_game();
	}
	catch (...)
	{
	}

	gameFilePath.clear();
}

void CoreBase::UnloadGame()
{
	UnloadGameNoDeinit();

	if (isInitialized)
	{
		retro_deinit();
		isInitialized = false;
	}
}

void CoreBase::RunFrame()
{
	try
	{
		retro_run();
	}
	catch (...)
	{
		throw ref new Platform::FailureException(L"Core runtime error");
	}
}

void CoreBase::Reset()
{
	retro_reset();
}

bool CoreBase::Serialize(WriteOnlyArray<uint8>^ stateData)
{
	return retro_serialize(stateData->Data, stateData->Length);
}

bool CoreBase::Unserialize(const Array<uint8>^ stateData)
{
	return retro_unserialize(stateData->Data, stateData->Length);
}