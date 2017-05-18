#pragma once

#include "Export.h"
#include "../LibretroRT/libretro.h"

namespace LibretroRTSupport
{
	class SUPPORT_API Converter
	{
	public:
		static Platform::String^ CToPlatformString(const char* t_str);
		static LibretroRT::GameGeometry^ CToRTGameGeometry(const retro_game_geometry& geometry);
		static LibretroRT::SystemTiming^ CToRTSystemTiming(const retro_system_timing& timing);
	};
}