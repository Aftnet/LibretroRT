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

void CoreBase::RaisePollInput()
{
	PollInput();
}
