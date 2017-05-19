#include "pch.h"
#include "CoreBase.h"
#include"Converter.h"
#include "../LibretroRT/libretro.h"

using namespace LibretroRTSupport;

CoreBase::CoreBase()
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
	PollInput();
}

size_t CoreBase::RaiseRenderAudioFrames(const int16_t * data, size_t frames)
{
	auto dataPtr = const_cast<int16_t*>(data);
	auto array = ref new Platform::Array<int16_t>(dataPtr, frames * 2);
	RenderAudioFrames(array, frames);
	return 0;
}
