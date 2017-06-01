#include "pch.h"
#include "CoreBase.h"
#include"Converter.h"
#include "../LibretroRT/libretro.h"

using namespace LibretroRTSupport;

void LogHandler(enum retro_log_level level, const char *fmt, ...)
{
#ifdef DEBUG
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

CoreBase::CoreBase() :
	timing(ref new SystemTiming),
	geometry(ref new GameGeometry),
	gameStream(nullptr),
	pixelFormat(LibretroRT::PixelFormats::FormatRGB565),
	CoreSystemPath(Converter::PlatformToCPPString(Windows::ApplicationModel::Package::Current->InstalledLocation->Path)),
	CoreSaveGamePath(Converter::PlatformToCPPString(Windows::Storage::ApplicationData::Current->LocalFolder->Path))
{
}

CoreBase::~CoreBase()
{
}

void CoreBase::SetSystemInfo(retro_system_info& info)
{
	name = Converter::CToPlatformString(info.library_name);
	version = Converter::CToPlatformString(info.library_version);
	supportedExtensions = Converter::CToPlatformString(info.valid_extensions);
}

void CoreBase::SetAVInfo(retro_system_av_info & info)
{
	geometry = Converter::CToRTGameGeometry(info.geometry);
	timing = Converter::CToRTSystemTiming(info.timing);
}

retro_game_info CoreBase::GenerateGameInfo(String^ gamePath, unsigned long long gameSize)
{
	static auto gamePathStr = Converter::PlatformToCPPString(gamePath);
	retro_game_info gameInfo;
	gameInfo.data = nullptr;
	gameInfo.path = gamePathStr.c_str();
	gameInfo.size = gameSize;
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

void CoreBase::ReadFileToMemory(std::vector<unsigned char>& data, IStorageFile^ file)
{
	auto stream = concurrency::create_task(file->OpenAsync(FileAccessMode::Read)).get();
	data.resize(stream->Size);
	auto dataArray = Platform::ArrayReference<unsigned char>(data.data(), stream->Size);

	auto reader = ref new Windows::Storage::Streams::DataReader(stream);
	concurrency::create_task(reader->LoadAsync(stream->Size)).get();
	reader->ReadBytes(dataArray);
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
		pixelFormat = Converter::ConvertToPixelFormat(*pix);
		PixelFormatChanged(pixelFormat);
		return true;
	}
	case RETRO_ENVIRONMENT_SET_GEOMETRY:
	{
		auto geom = reinterpret_cast<retro_game_geometry*>(data);
		geometry = Converter::CToRTGameGeometry(*geom);
		GameGeometryChanged(geometry);
		return true;
	}
	case RETRO_ENVIRONMENT_SET_SYSTEM_AV_INFO:
	{
		auto avInfo = reinterpret_cast<retro_system_av_info*>(data);
		SetAVInfo(*avInfo);
		GameGeometryChanged(geometry);
		SystemTimingChanged(timing);
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

size_t CoreBase::ReadGameFileHandler(void* buffer, size_t requested)
{
	if (gameStream == nullptr)
		return 0;

	auto remaining = (size_t)(gameStream->Size - gameStream->Position);
	requested = min(requested, remaining);

	auto arrayRef = ArrayReference<UCHAR>((UCHAR*)buffer, requested, false);
	auto reader = ref new Streams::DataReader(gameStream);
	concurrency::create_task(reader->LoadAsync(requested)).wait();
	reader->ReadBytes(arrayRef);

	return requested;
}

void CoreBase::SeekGameFileHandler(unsigned long position)
{
	if (gameStream == nullptr)
		return;

	gameStream->Seek(position);
}

void CoreBase::RaisePollInput()
{
	PollInput();
}

int16_t CoreBase::RaiseGetInputState(unsigned port, unsigned device, unsigned index, unsigned keyId)
{
	auto key = Converter::ConvertToInputType(device, index, keyId);
	return GetInputState(port, key);
}

size_t CoreBase::RaiseRenderAudioFrames(const int16_t* data, size_t frames)
{
	auto dataPtr = const_cast<int16_t*>(data);
	auto dataArray = Platform::ArrayReference<int16_t>(dataPtr, frames * 2);
	RenderAudioFrames(dataArray);
	return 0;
}

void CoreBase::RaiseRenderVideoFrame(const void* data, unsigned width, unsigned height, size_t pitch)
{
	auto dataPtr = reinterpret_cast<uint8*>(const_cast<void*>(data));
	//See retro_video_refresh_t for why buffer size is computed that way
	auto dataArray = Platform::ArrayReference<uint8>(dataPtr, height * pitch);
	RenderVideoFrame(dataArray, width, height, pitch);
}
