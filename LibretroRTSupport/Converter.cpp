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
		switch (index)
		{
		case RETRO_DEVICE_INDEX_ANALOG_LEFT:
			switch (id)
			{
			case RETRO_DEVICE_ID_ANALOG_X:
				return InputType::DeviceIdAnalogLeftX;
			case RETRO_DEVICE_ID_ANALOG_Y:
				return InputType::DeviceIdAnalogLeftY;
			default:
				return InputType::DeviceIdUnknown;
			}
		case RETRO_DEVICE_INDEX_ANALOG_RIGHT:
			switch (id)
			{
			case RETRO_DEVICE_ID_ANALOG_X:
				return InputType::DeviceIdAnalogRightX;
			case RETRO_DEVICE_ID_ANALOG_Y:
				return InputType::DeviceIdAnalogRightY;
			default:
				return InputType::DeviceIdUnknown;
			}
		default:
			return InputType::DeviceIdUnknown;
		}
	case RETRO_DEVICE_MOUSE:
		switch (id)
		{
		case RETRO_DEVICE_ID_MOUSE_X:
			return InputType::DeviceIdMouseX;
		case RETRO_DEVICE_ID_MOUSE_Y:
			return InputType::DeviceIdMouseY;
		case RETRO_DEVICE_ID_MOUSE_LEFT:
			return InputType::DeviceIdMouseLeft;
		case RETRO_DEVICE_ID_MOUSE_RIGHT:
			return InputType::DeviceIdMouseRight;
		case RETRO_DEVICE_ID_MOUSE_WHEELUP:
			return InputType::DeviceIdMouseWheelup;
		case RETRO_DEVICE_ID_MOUSE_WHEELDOWN:
			return InputType::DeviceIdMouseWheeldown;
		case RETRO_DEVICE_ID_MOUSE_MIDDLE:
			return InputType::DeviceIdMouseMiddle;
		case RETRO_DEVICE_ID_MOUSE_HORIZ_WHEELUP:
			return InputType::DeviceIdMouseHorizWheelup;
		case RETRO_DEVICE_ID_MOUSE_HORIZ_WHEELDOWN:
			return InputType::DeviceIdMouseHorizWheeldown;
		default:
			return InputType::DeviceIdUnknown;
		}
	case RETRO_DEVICE_LIGHTGUN:
		switch (id)
		{
		case RETRO_DEVICE_ID_LIGHTGUN_X:
			return InputType::DeviceIdLightgunX;
		case RETRO_DEVICE_ID_LIGHTGUN_Y:
			return InputType::DeviceIdLightgunY;
		case RETRO_DEVICE_ID_LIGHTGUN_TRIGGER:
			return InputType::DeviceIdLightgunTrigger;
		case RETRO_DEVICE_ID_LIGHTGUN_CURSOR:
			return InputType::DeviceIdLightgunCursor;
		case RETRO_DEVICE_ID_LIGHTGUN_TURBO:
			return InputType::DeviceIdLightgunTurbo;
		case RETRO_DEVICE_ID_LIGHTGUN_PAUSE:
			return InputType::DeviceIdLightgunPause;
		case RETRO_DEVICE_ID_LIGHTGUN_START:
			return InputType::DeviceIdLightgunStart;
		default:
			return InputType::DeviceIdUnknown;
		}
	case RETRO_DEVICE_POINTER:
		switch (id)
		{
		case RETRO_DEVICE_ID_POINTER_X:
			return InputType::DeviceIdPointerX;
		case RETRO_DEVICE_ID_POINTER_Y:
			return InputType::DeviceIdPointerY;
		case RETRO_DEVICE_ID_POINTER_PRESSED:
			return InputType::DeviceIdPointerPressed;
		default:
			return InputType::DeviceIdUnknown;
		}
	default:
		return InputType::DeviceIdUnknown;
	}
}
