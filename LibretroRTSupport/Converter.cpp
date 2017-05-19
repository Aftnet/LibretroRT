#include "pch.h"
#include "Converter.h"
#include "../LibretroRT/libretro.h"

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

InputType LibretroRTSupport::Converter::ConvertToInputType(unsigned device, unsigned index, unsigned id)
{
	switch (device)
	{
	case RETRO_DEVICE_JOYPAD:
		switch (id)
		{
		case RETRO_DEVICE_ID_JOYPAD_B:
			return InputType::DeviceIdJoypadB;
		case RETRO_DEVICE_ID_JOYPAD_Y:
			return InputType::DeviceIdJoypadY;
		case RETRO_DEVICE_ID_JOYPAD_SELECT:
			return InputType::DeviceIdJoypadSelect;
		case RETRO_DEVICE_ID_JOYPAD_START:
			return InputType::DeviceIdJoypadStart;
		case RETRO_DEVICE_ID_JOYPAD_UP:
			return InputType::DeviceIdJoypadUp;
		case RETRO_DEVICE_ID_JOYPAD_DOWN:
			return InputType::DeviceIdJoypadDown;
		case RETRO_DEVICE_ID_JOYPAD_LEFT:
			return InputType::DeviceIdJoypadLeft;
		case RETRO_DEVICE_ID_JOYPAD_RIGHT:
			return InputType::DeviceIdJoypadRight;
		case RETRO_DEVICE_ID_JOYPAD_A:
			return InputType::DeviceIdJoypadA;
		case RETRO_DEVICE_ID_JOYPAD_X:
			return InputType::DeviceIdJoypadX;
		case RETRO_DEVICE_ID_JOYPAD_L:
			return InputType::DeviceIdJoypadL;
		case RETRO_DEVICE_ID_JOYPAD_R:
			return InputType::DeviceIdJoypadR;
		case RETRO_DEVICE_ID_JOYPAD_L2:
			return InputType::DeviceIdJoypadL2;
		case RETRO_DEVICE_ID_JOYPAD_R2:
			return InputType::DeviceIdJoypadR2;
		case RETRO_DEVICE_ID_JOYPAD_L3:
			return InputType::DeviceIdJoypadL3;
		case RETRO_DEVICE_ID_JOYPAD_R3:
			return InputType::DeviceIdJoypadR3;
		default:
			return InputType::DeviceIdUnknown;
		}
	case RETRO_DEVICE_ANALOG:
		switch (id)
		{
		default:
			return InputType::DeviceIdUnknown;
		}
	case RETRO_DEVICE_MOUSE:
		switch (id)
		{
		default:
			return InputType::DeviceIdUnknown;
		}
	case RETRO_DEVICE_LIGHTGUN:
		switch (id)
		{
		default:
			return InputType::DeviceIdUnknown;
		}
	case RETRO_DEVICE_POINTER:
		switch (id)
		{
		default:
			return InputType::DeviceIdUnknown;
		}
	default:
		return InputType::DeviceIdUnknown;
	}
}
