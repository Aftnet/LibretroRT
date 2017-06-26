#pragma once

#include "Export.h"
#include <codecvt>

struct retro_game_geometry;
struct retro_system_timing;
enum retro_pixel_format;

using namespace Platform;
using namespace LibretroRT;

namespace LibretroRT_Tools
{
	class SUPPORT_API Converter
	{
	public:
		static String^ CPPToPlatformString(const std::string string);
		static std::string PlatformToCPPString(Platform::String^ string);
		static std::vector<std::string> SplitString(const std::string input, char delimiter);
		static GameGeometry^ CToRTGameGeometry(const retro_game_geometry& geometry);
		static SystemTiming^ CToRTSystemTiming(const retro_system_timing& timing);
		static PixelFormats ConvertToPixelFormat(enum retro_pixel_format format);
		static InputTypes ConvertToInputType(unsigned device, unsigned index, unsigned id);
	private:

		static std::wstring_convert<std::codecvt_byname<wchar_t, char, std::mbstate_t>> StringConverter;
	};
}