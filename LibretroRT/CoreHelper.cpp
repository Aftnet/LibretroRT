#include "pch.h"
#include "CoreHelper.h"
#include "Converter.h"
#include "GameGeometry.h"
#include "SystemTiming.h"

#include "libretro.h"

using namespace LibretroRT;

CoreHelper::CoreHelper(retro_system_info& info):
	name(Converter::CToPlatformString(info.library_name)),
	version(Converter::CToPlatformString(info.library_version)),
	supportedExtensions(Converter::CToPlatformString(info.valid_extensions))
{
}

void CoreHelper::SetAVInfo(retro_system_av_info & avInfo)
{
	gameGeometry = ref new LibretroRT::GameGeometry(avInfo.geometry);
	systemTiming = ref new LibretroRT::SystemTiming(avInfo.timing);
}
