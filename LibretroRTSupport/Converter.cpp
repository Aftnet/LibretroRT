#include "pch.h"
#include "Converter.h"
#include "../LibretroRT/libretro.h"

using namespace Platform;
using namespace LibretroRT;
using namespace LibretroRTSupport;

Platform::String^ Converter::CToPlatformString(const char* t_str)
{
	//setup converter
	typedef std::codecvt_utf8<wchar_t> convert_type;
	std::wstring_convert<convert_type, wchar_t> converter;

	//use converter (.to_bytes: wstr->str, .from_bytes: str->wstr)
	auto wstring = converter.from_bytes(t_str);
	return ref new String(wstring.c_str());
}

GameGeometry^ Converter::CToRTGameGeometry(const retro_game_geometry & geometry)
{
	return ref new GameGeometry(geometry.base_width, geometry.base_height, geometry.max_width, geometry.max_height, geometry.aspect_ratio);
}

SystemTiming^ Converter::CToRTSystemTiming(const retro_system_timing & timing)
{
	return ref new SystemTiming(timing.fps, timing.sample_rate);
}

PixelFormats Converter::ConvertToPixelFormat(enum retro_pixel_format format)
{
	switch (format)
	{
	case RETRO_PIXEL_FORMAT_0RGB1555:
		return PixelFormats::Format0RGB1555;
	case RETRO_PIXEL_FORMAT_XRGB8888:
		return PixelFormats::FormatXRGB8888;
	case RETRO_PIXEL_FORMAT_RGB565:
		return PixelFormats::FormatRGB565;
	default:
		return PixelFormats::FormatUknown;
	}
}

InputTypes Converter::ConvertToInputType(unsigned device, unsigned index, unsigned id)
{
	switch (device)
	{
	case RETRO_DEVICE_JOYPAD:
		switch (id)
		{
		case RETRO_DEVICE_ID_JOYPAD_B:
			return InputTypes::DeviceIdJoypadB;
		case RETRO_DEVICE_ID_JOYPAD_Y:
			return InputTypes::DeviceIdJoypadY;
		case RETRO_DEVICE_ID_JOYPAD_SELECT:
			return InputTypes::DeviceIdJoypadSelect;
		case RETRO_DEVICE_ID_JOYPAD_START:
			return InputTypes::DeviceIdJoypadStart;
		case RETRO_DEVICE_ID_JOYPAD_UP:
			return InputTypes::DeviceIdJoypadUp;
		case RETRO_DEVICE_ID_JOYPAD_DOWN:
			return InputTypes::DeviceIdJoypadDown;
		case RETRO_DEVICE_ID_JOYPAD_LEFT:
			return InputTypes::DeviceIdJoypadLeft;
		case RETRO_DEVICE_ID_JOYPAD_RIGHT:
			return InputTypes::DeviceIdJoypadRight;
		case RETRO_DEVICE_ID_JOYPAD_A:
			return InputTypes::DeviceIdJoypadA;
		case RETRO_DEVICE_ID_JOYPAD_X:
			return InputTypes::DeviceIdJoypadX;
		case RETRO_DEVICE_ID_JOYPAD_L:
			return InputTypes::DeviceIdJoypadL;
		case RETRO_DEVICE_ID_JOYPAD_R:
			return InputTypes::DeviceIdJoypadR;
		case RETRO_DEVICE_ID_JOYPAD_L2:
			return InputTypes::DeviceIdJoypadL2;
		case RETRO_DEVICE_ID_JOYPAD_R2:
			return InputTypes::DeviceIdJoypadR2;
		case RETRO_DEVICE_ID_JOYPAD_L3:
			return InputTypes::DeviceIdJoypadL3;
		case RETRO_DEVICE_ID_JOYPAD_R3:
			return InputTypes::DeviceIdJoypadR3;
		default:
			return InputTypes::DeviceIdUnknown;
		}
	case RETRO_DEVICE_ANALOG:
		switch (index)
		{
		case RETRO_DEVICE_INDEX_ANALOG_LEFT:
			switch (id)
			{
			case RETRO_DEVICE_ID_ANALOG_X:
				return InputTypes::DeviceIdAnalogLeftX;
			case RETRO_DEVICE_ID_ANALOG_Y:
				return InputTypes::DeviceIdAnalogLeftY;
			default:
				return InputTypes::DeviceIdUnknown;
			}
		case RETRO_DEVICE_INDEX_ANALOG_RIGHT:
			switch (id)
			{
			case RETRO_DEVICE_ID_ANALOG_X:
				return InputTypes::DeviceIdAnalogRightX;
			case RETRO_DEVICE_ID_ANALOG_Y:
				return InputTypes::DeviceIdAnalogRightY;
			default:
				return InputTypes::DeviceIdUnknown;
			}
		default:
			return InputTypes::DeviceIdUnknown;
		}
	case RETRO_DEVICE_MOUSE:
		switch (id)
		{
		case RETRO_DEVICE_ID_MOUSE_X:
			return InputTypes::DeviceIdMouseX;
		case RETRO_DEVICE_ID_MOUSE_Y:
			return InputTypes::DeviceIdMouseY;
		case RETRO_DEVICE_ID_MOUSE_LEFT:
			return InputTypes::DeviceIdMouseLeft;
		case RETRO_DEVICE_ID_MOUSE_RIGHT:
			return InputTypes::DeviceIdMouseRight;
		case RETRO_DEVICE_ID_MOUSE_WHEELUP:
			return InputTypes::DeviceIdMouseWheelup;
		case RETRO_DEVICE_ID_MOUSE_WHEELDOWN:
			return InputTypes::DeviceIdMouseWheeldown;
		case RETRO_DEVICE_ID_MOUSE_MIDDLE:
			return InputTypes::DeviceIdMouseMiddle;
		case RETRO_DEVICE_ID_MOUSE_HORIZ_WHEELUP:
			return InputTypes::DeviceIdMouseHorizWheelup;
		case RETRO_DEVICE_ID_MOUSE_HORIZ_WHEELDOWN:
			return InputTypes::DeviceIdMouseHorizWheeldown;
		default:
			return InputTypes::DeviceIdUnknown;
		}
	case RETRO_DEVICE_LIGHTGUN:
		switch (id)
		{
		case RETRO_DEVICE_ID_LIGHTGUN_X:
			return InputTypes::DeviceIdLightgunX;
		case RETRO_DEVICE_ID_LIGHTGUN_Y:
			return InputTypes::DeviceIdLightgunY;
		case RETRO_DEVICE_ID_LIGHTGUN_TRIGGER:
			return InputTypes::DeviceIdLightgunTrigger;
		case RETRO_DEVICE_ID_LIGHTGUN_CURSOR:
			return InputTypes::DeviceIdLightgunCursor;
		case RETRO_DEVICE_ID_LIGHTGUN_TURBO:
			return InputTypes::DeviceIdLightgunTurbo;
		case RETRO_DEVICE_ID_LIGHTGUN_PAUSE:
			return InputTypes::DeviceIdLightgunPause;
		case RETRO_DEVICE_ID_LIGHTGUN_START:
			return InputTypes::DeviceIdLightgunStart;
		default:
			return InputTypes::DeviceIdUnknown;
		}
	case RETRO_DEVICE_POINTER:
		switch (id)
		{
		case RETRO_DEVICE_ID_POINTER_X:
			return InputTypes::DeviceIdPointerX;
		case RETRO_DEVICE_ID_POINTER_Y:
			return InputTypes::DeviceIdPointerY;
		case RETRO_DEVICE_ID_POINTER_PRESSED:
			return InputTypes::DeviceIdPointerPressed;
		default:
			return InputTypes::DeviceIdUnknown;
		}
	default:
		return InputTypes::DeviceIdUnknown;
	}
}
