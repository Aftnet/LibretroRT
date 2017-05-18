#include "pch.h"
#include "Converter.h"

Platform::String^ LibretroRTSupport::Converter::CToPlatformString(const char* t_str)
{
	//setup converter
	typedef std::codecvt_utf8<wchar_t> convert_type;
	std::wstring_convert<convert_type, wchar_t> converter;

	//use converter (.to_bytes: wstr->str, .from_bytes: str->wstr)
	auto wstring = converter.from_bytes(t_str);
	return ref new Platform::String(wstring.c_str());
}

LibretroRT::GameGeometry^ LibretroRTSupport::Converter::CToRTGameGeometry(const retro_game_geometry & geometry)
{
	return ref new LibretroRT::GameGeometry(geometry.base_width, geometry.base_height, geometry.max_width, geometry.max_height, geometry.aspect_ratio);
}

LibretroRT::SystemTiming ^ LibretroRTSupport::Converter::CToRTSystemTiming(const retro_system_timing & timing)
{
	return ref new LibretroRT::SystemTiming(timing.fps, timing.sample_rate);
}
