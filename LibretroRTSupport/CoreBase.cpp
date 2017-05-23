#include "pch.h"
#include "CoreBase.h"
#include"Converter.h"
#include "../LibretroRT/libretro.h"

using namespace LibretroRTSupport;

CoreBase::CoreBase():
	timing(ref new SystemTiming),
	geometry(ref new GameGeometry),
	pixelFormat(LibretroRT::PixelFormats::FormatUknown),
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

bool CoreBase::EnvironmentHandler(unsigned cmd, void *data)
{
	switch (cmd)
	{
	case RETRO_ENVIRONMENT_GET_SYSTEM_DIRECTORY:
	{
		auto dataPtr = (const char**)data;
		*dataPtr = CoreSystemPath.c_str();
		return true;
	}
	case RETRO_ENVIRONMENT_GET_SAVE_DIRECTORY:
	{
		auto dataPtr = (const char**)data;
		*dataPtr = CoreSaveGamePath.c_str();
		return true;
	}
	case RETRO_ENVIRONMENT_SET_PIXEL_FORMAT:
		auto pix = reinterpret_cast<enum retro_pixel_format*>(data);
		pixelFormat = Converter::ConvertToPixelFormat(*pix);
		return true;
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

	auto arrayRef = ArrayReference<UCHAR>((UCHAR*)buffer, requested, false);
	auto reader = ref new Streams::DataReader(gameStream);
	reader->ReadBytes(arrayRef);

	auto remaining = gameStream->Size - gameStream->Position;
	return min(requested, remaining);
}

void CoreBase::SeekGameFileHandler(unsigned long requested)
{
	if (gameStream == nullptr)
		return;

	gameStream->Seek(requested);
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
	RenderAudioFrames(dataArray, frames);
	return 0;
}

void CoreBase::RaiseRenderVideoFrame(const void* data, unsigned width, unsigned height, size_t pitch)
{
	auto dataPtr = reinterpret_cast<uint8*>(const_cast<void*>(data));
	//See retro_video_refresh_t for why buffer size is computed that way
	auto dataArray = Platform::ArrayReference<uint8>(dataPtr, height * pitch);
	RenderVideoFrame(dataArray, width, height, pitch);
}
