#pragma once

#include "Export.h"

struct retro_game_geometry;
struct retro_system_timing;

using namespace Platform;
using namespace LibretroRT;

namespace LibretroRTSupport
{
	class SUPPORT_API Converter
	{
	public:
		static String^ CToPlatformString(const char* t_str);
		static GameGeometry^ CToRTGameGeometry(const retro_game_geometry& geometry);
		static SystemTiming^ CToRTSystemTiming(const retro_system_timing& timing);
		static InputType ConvertToInputType(unsigned device, unsigned index, unsigned id);
	};
}