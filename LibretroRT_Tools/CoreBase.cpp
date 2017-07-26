#include "pch.h"
#include "CoreBase.h"
#include "Converter.h"
#include "StringConverter.h"
#include "../LibretroRT/libretro.h"

using namespace LibretroRT_Tools;
using namespace Windows::Storage;

void LogHandler(enum retro_log_level level, const char *fmt, ...)
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
}

CoreBase::CoreBase(LibretroGetSystemInfoPtr libretroGetSystemInfo, LibretroGetSystemAVInfoPtr libretroGetSystemAVInfo,
	LibretroLoadGamePtr libretroLoadGame, LibretroUnloadGamePtr libretroUnloadGame,
	LibretroRunPtr libretroRun, LibretroResetPtr libretroReset, LibretroSerializeSizePtr libretroSerializeSize,
	LibretroSerializePtr libretroSerialize, LibretroUnserializePtr libretroUnserialize, LibretroDeinitPtr libretroDeinit,
	bool supportsSystemFolderVirtualization, bool supportsSaveGameFolderVirtualization) :
	LibretroGetSystemInfo(libretroGetSystemInfo),
	LibretroGetSystemAVInfo(libretroGetSystemAVInfo),
	LibretroLoadGame(libretroLoadGame),
	LibretroUnloadGame(libretroUnloadGame),
	LibretroRun(libretroRun),
	LibretroReset(libretroReset),
	LibretroSerializeSize(libretroSerializeSize),
	LibretroSerialize(libretroSerialize),
	LibretroUnserialize(libretroUnserialize),
	LibretroDeinit(libretroDeinit),
	supportsSystemFolderVirtualization(supportsSystemFolderVirtualization),
	supportsSaveGameFolderVirtualization(supportsSaveGameFolderVirtualization),
	options(ref new Map<String^, CoreOption^>()),
	pixelFormat(LibretroRT::PixelFormats::FormatRGB565),
	geometry(ref new GameGeometry),
	timing(ref new SystemTiming),
	coreRequiresGameFilePath(true),
	systemFolder(nullptr),
	saveGameFolder(nullptr),
	fileDependencies(ref new Vector<FileDependency^>)
{
	retro_system_info info;
	LibretroGetSystemInfo(&info);

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
	LibretroDeinit();
}

void CoreBase::ReadFileToMemory(String^ filePath, std::vector<unsigned char>& data)
{
	auto stream = OpenFileStream(filePath, Windows::Storage::FileAccessMode::Read);
	data.resize(stream->Size);
	auto dataArray = Platform::ArrayReference<unsigned char>(data.data(), stream->Size);

	auto reader = ref new Windows::Storage::Streams::DataReader(stream);
	concurrency::create_task(reader->LoadAsync(stream->Size)).get();
	reader->ReadBytes(dataArray);
	CloseFileStream(stream);
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
		dataPtr->log = LogHandler;
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

bool CoreBase::LoadGame(String^ mainGameFilePath)
{
	if (!gameFilePath.empty())
	{
		UnloadGame();
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
			ReadFileToMemory(mainGameFilePath, gameData);
			gameInfo.data = gameData.data();
			gameInfo.size = gameData.size();
		}

		auto loadSuccessful = LibretroLoadGame(&gameInfo);
		if (loadSuccessful)
		{
			retro_system_av_info info;
			LibretroGetSystemAVInfo(&info);

			Geometry = Converter::CToRTGameGeometry(info.geometry);
			Timing = Converter::CToRTSystemTiming(info.timing);
		}	
		else
		{
			gameFilePath.clear();
		}
	}
	catch (const std::exception& e)
	{
		UnloadGame();
	}

	return !gameFilePath.empty();
}

void CoreBase::UnloadGame()
{
	if (gameFilePath.empty())
	{
		return;
	}

	try
	{
		LibretroUnloadGame();
	}
	catch (...)
	{
		//throw ref new Platform::FailureException(L"Core runtime error");
	}

	gameFilePath.clear();
}

void CoreBase::RunFrame()
{
	try
	{
		LibretroRun();
	}
	catch (...)
	{
		//throw ref new Platform::FailureException(L"Core runtime error");
	}
}

void CoreBase::Reset()
{
	LibretroReset();
}

bool CoreBase::Serialize(WriteOnlyArray<uint8>^ stateData)
{
	return LibretroSerialize(stateData->Data, stateData->Length);
}

bool CoreBase::Unserialize(const Array<uint8>^ stateData)
{
	return LibretroUnserialize(stateData->Data, stateData->Length);
}