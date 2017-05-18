#include "pch.h"
#include "CoreHelper.h"
#include "Converter.h"
#include "../LibretroRT/libretro.h"

LibretroRTSupport::CoreHelper::CoreHelper(const retro_system_info & info):
	name(Converter::CToPlatformString(info.library_name)),
	version(Converter::CToPlatformString(info.library_version)),
	supportedExtensions(Converter::CToPlatformString(info.valid_extensions))
{
}
