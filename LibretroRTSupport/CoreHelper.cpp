#include "pch.h"
#include "CoreHelper.h"
#include "Converter.h"
#include "../LibretroRT/libretro.h"

LibretroRTSupport::CoreHelper::CoreHelper(const retro_system_info & info):
	name(Converter::CToPlatformString(info.library_name)),
	version(Converter::CToPlatformString(info.library_version)),
	supportedExtensions(Converter::CToPlatformString(info.valid_extensions)),
	gameGeometry(ref new GameGeometry()),
	systemTiming(ref new SystemTiming())
{
}

void LibretroRTSupport::CoreHelper::SetAVInfo(const retro_system_av_info & info)
{
	gameGeometry = Converter::CToRTGameGeometry(info.geometry);
	systemTiming = Converter::CToRTSystemTiming(info.timing);
}

bool LibretroRTSupport::CoreHelper::DefaultEnvironmentHandler(unsigned cmd, void * data)
{
	return false;
}