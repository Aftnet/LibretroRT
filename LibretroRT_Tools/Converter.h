#pragma once

#include "Export.h"

struct retro_game_geometry;
struct retro_system_timing;
struct retro_variable;
enum retro_pixel_format;

using namespace Platform;
using namespace LibretroRT;

namespace LibretroRT_Tools
{
	class SUPPORT_API Converter
	{
	public:
		static GameGeometry^ CToRTGameGeometry(const retro_game_geometry& geometry);
		static SystemTiming^ CToRTSystemTiming(const retro_system_timing& timing);
		static CoreOptionDescription^ RetroVariableToCoreOptionDescription(const retro_variable& variable);
		static PixelFormats ConvertToPixelFormat(enum retro_pixel_format format);
		static InputTypes ConvertToInputType(unsigned device, unsigned index, unsigned id);
	};
}