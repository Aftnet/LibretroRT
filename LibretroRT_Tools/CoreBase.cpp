#include "pch.h"
#include "CoreBase.h"
#include "Converter.h"
#include "../LibretroRT/libretro.h"

using namespace LibretroRT_Tools;

void LogHandler(enum retro_log_level level, const char *fmt, ...)
{
#ifndef NDEBUG
	const int bufLen = 1024;
	static char logBuffer[bufLen];

	va_list args;
	va_start(args, fmt);
	vsnprintf_s(logBuffer, bufLen, fmt, args);
	va_end(args);

	auto debugMsg = Converter::CToWString(logBuffer);
	OutputDebugString(debugMsg.c_str());
#endif // DEBUG
}

CoreBase::CoreBase(LibretroGetSystemInfoPtr libretroGetSystemInfo, LibretroGetSystemAVInfoPtr libretroGetSystemAVInfo,
	LibretroLoadGamePtr libretroLoadGame, LibretroUnloadGamePtr libretroUnloadGame,
	LibretroRunPtr libretroRun, LibretroResetPtr libretroReset, LibretroSerializeSizePtr libretroSerializeSize,
	LibretroSerializePtr libretroSerialize, LibretroUnserializePtr libretroUnserialize, LibretroDeinitPtr libretroDeinit) :
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
	timing(ref new SystemTiming),
	geometry(ref new GameGeometry),
	gameLoaded(false),
	coreRequiresGameFilePath(true),
	pixelFormat(LibretroRT::PixelFormats::FormatRGB565),
	CoreSystemPath(Converter::PlatformToCPPString(Windows::ApplicationModel::Package::Current->InstalledLocation->Path)),
	CoreSaveGamePath(Converter::PlatformToCPPString(Windows::Storage::ApplicationData::Current->LocalFolder->Path))
{
	retro_system_info info;
	LibretroGetSystemInfo(&info);

	name = Converter::CToPlatformString(info.library_name);
	version = Converter::CToPlatformString(info.library_version);
	supportedExtensions = Converter::CToPlatformString(info.valid_extensions);
	coreRequiresGameFilePath = info.need_fullpath;
}

CoreBase::~CoreBase()
{
	LibretroDeinit();
}

retro_game_info CoreBase::GenerateGameInfo(String^ gamePath)
{
	static auto gamePathStr = Converter::PlatformToCPPString(gamePath);
	retro_game_info gameInfo;
	gameInfo.data = nullptr;
	gameInfo.path = gamePathStr.c_str();
	gameInfo.size = 0;
	gameInfo.meta = nullptr;
	return gameInfo;
}

retro_game_info CoreBase::GenerateGameInfo(const std::vector<unsigned char>& gameData)
{
	retro_game_info gameInfo;
	gameInfo.data = gameData.data();
	gameInfo.path = nullptr;
	gameInfo.size = gameData.size();
	gameInfo.meta = nullptr;
	return gameInfo;
}

void CoreBase::ReadFileToMemory(String^ filePath, std::vector<unsigned char>& data)
{
	auto stream = GetFileStream(filePath, Windows::Storage::FileAccessMode::Read);
	data.resize(stream->Size);
	auto dataArray = Platform::ArrayReference<unsigned char>(data.data(), stream->Size);

	auto reader = ref new Windows::Storage::Streams::DataReader(stream);
	concurrency::create_task(reader->LoadAsync(stream->Size)).get();
	reader->ReadBytes(dataArray);
	stream = nullptr;
}

bool CoreBase::EnvironmentHandler(unsigned cmd, void *data)
{
	switch (cmd)
	{
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
		*dataPtr = CoreSystemPath.c_str();
		return true;
	}
	case RETRO_ENVIRONMENT_GET_SAVE_DIRECTORY:
	{
		auto dataPtr = reinterpret_cast<const char**>(data);
		*dataPtr = CoreSaveGamePath.c_str();
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
		auto pix = reinterpret_cast<enum retro_pixel_format*>(data);
		PixelFormat = Converter::ConvertToPixelFormat(*pix);
		return true;
	}
	case RETRO_ENVIRONMENT_SET_GEOMETRY:
	{
		auto geom = reinterpret_cast<retro_game_geometry*>(data);
		Geometry = Converter::CToRTGameGeometry(*geom);
		return true;
	}
	case RETRO_ENVIRONMENT_SET_SYSTEM_AV_INFO:
	{
		auto avInfo = reinterpret_cast<retro_system_av_info*>(data);
		Geometry = Converter::CToRTGameGeometry(avInfo->geometry);
		Timing = Converter::CToRTSystemTiming(avInfo->timing);
		return true;
	}
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

	auto dataPtr = reinterpret_cast<uint8*>(const_cast<void*>(data));
	//See retro_video_refresh_t for why buffer size is computed that way
	auto dataArray = Platform::ArrayReference<uint8>(dataPtr, height * pitch);
	RenderVideoFrame(dataArray, width, height, pitch);
}

bool CoreBase::LoadGame(String^ mainGameFilePath)
{
	if (gameLoaded)
	{
		UnloadGame();
	}

	try
	{
		retro_game_info gameInfo = GenerateGameInfo(mainGameFilePath);
		if (!coreRequiresGameFilePath)
		{
			//gameInfo = GenerateGameInfo(gameData);
		}

		gameLoaded = retro_load_game(&gameInfo);
		if (gameLoaded)
		{
			retro_system_av_info info;
			LibretroGetSystemAVInfo(&info);

			Geometry = Converter::CToRTGameGeometry(info.geometry);
			Timing = Converter::CToRTSystemTiming(info.timing);
		}		
	}
	catch (const std::exception& e)
	{
		UnloadGame();
		gameLoaded = false;
	}

	return gameLoaded;
}

void CoreBase::UnloadGame()
{
	if (!gameLoaded)
	{
		return;
	}

	LibretroUnloadGame();
	gameLoaded = false;
}

void CoreBase::RunFrame()
{
	try
	{
		LibretroRun();
	}
	catch (...)
	{
		throw ref new Platform::FailureException(L"Core runtime error");
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