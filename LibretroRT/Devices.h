#pragma once

#include "libretro.h"

namespace LibretroRT
{
	namespace Devices
	{
		public enum class DeviceType
		{
			None = RETRO_DEVICE_NONE,
			Joypad = RETRO_DEVICE_ANALOG,
			Mouse = RETRO_DEVICE_MOUSE,
			Keyboard = RETRO_DEVICE_KEYBOARD,
			LightGun = RETRO_DEVICE_LIGHTGUN,
			Analog = RETRO_DEVICE_ANALOG,
			Pointer = RETRO_DEVICE_POINTER
		};

		const unsigned int DeviceIdJoypadB = RETRO_DEVICE_ID_JOYPAD_B;
		const unsigned int DeviceIdJoypadY = RETRO_DEVICE_ID_JOYPAD_Y;
		const unsigned int DeviceIdJoypadSelect = RETRO_DEVICE_ID_JOYPAD_SELECT;
		const unsigned int DeviceIdJoypadStart = RETRO_DEVICE_ID_JOYPAD_START;
		const unsigned int DeviceIdJoypadUp = RETRO_DEVICE_ID_JOYPAD_UP;
		const unsigned int DeviceIdJoypadDown = RETRO_DEVICE_ID_JOYPAD_DOWN;
		const unsigned int DeviceIdJoypadLeft = RETRO_DEVICE_ID_JOYPAD_LEFT;
		const unsigned int DeviceIdJoypadRight = RETRO_DEVICE_ID_JOYPAD_RIGHT;
		const unsigned int DeviceIdJoypadA = RETRO_DEVICE_ID_JOYPAD_A;
		const unsigned int DeviceIdJoypadX = RETRO_DEVICE_ID_JOYPAD_X;
		const unsigned int DeviceIdJoypadL = RETRO_DEVICE_ID_JOYPAD_L;
		const unsigned int DeviceIdJoypadR = RETRO_DEVICE_ID_JOYPAD_R;
		const unsigned int DeviceIdJoypadL2 = RETRO_DEVICE_ID_JOYPAD_L2;
		const unsigned int DeviceIdJoypadR2 = RETRO_DEVICE_ID_JOYPAD_R2;
		const unsigned int DeviceIdJoypadL3 = RETRO_DEVICE_ID_JOYPAD_L3;
		const unsigned int DeviceIdJoypadR3 = RETRO_DEVICE_ID_JOYPAD_R3;
		const unsigned int DeviceIndexAnalogLeft = RETRO_DEVICE_INDEX_ANALOG_LEFT;
		const unsigned int DeviceIndexAnalogRight = RETRO_DEVICE_INDEX_ANALOG_RIGHT;
		const unsigned int DeviceIdAnalogX = RETRO_DEVICE_ID_ANALOG_X;
		const unsigned int DeviceIdAnalogY = RETRO_DEVICE_ID_ANALOG_Y;
		const unsigned int DeviceIdMouseX = RETRO_DEVICE_ID_MOUSE_X;
		const unsigned int DeviceIdMouseY = RETRO_DEVICE_ID_MOUSE_Y;
		const unsigned int DeviceIdMouseLeft = RETRO_DEVICE_ID_MOUSE_LEFT;
		const unsigned int DeviceIdMouseRight = RETRO_DEVICE_ID_MOUSE_RIGHT;
		const unsigned int DeviceIdMouseWheelup = RETRO_DEVICE_ID_MOUSE_WHEELUP;
		const unsigned int DeviceIdMouseWheeldown = RETRO_DEVICE_ID_MOUSE_WHEELDOWN;
		const unsigned int DeviceIdMouseMiddle = RETRO_DEVICE_ID_MOUSE_MIDDLE;
		const unsigned int DeviceIdMouseHorizWheelup = RETRO_DEVICE_ID_MOUSE_HORIZ_WHEELUP;
		const unsigned int DeviceIdMouseHorizWheeldown = RETRO_DEVICE_ID_MOUSE_HORIZ_WHEELDOWN;
		const unsigned int DeviceIdLightgunX = RETRO_DEVICE_ID_LIGHTGUN_X;
		const unsigned int DeviceIdLightgunY = RETRO_DEVICE_ID_LIGHTGUN_Y;
		const unsigned int DeviceIdLightgunTrigger = RETRO_DEVICE_ID_LIGHTGUN_TRIGGER;
		const unsigned int DeviceIdLightgunCursor = RETRO_DEVICE_ID_LIGHTGUN_CURSOR;
		const unsigned int DeviceIdLightgunTurbo = RETRO_DEVICE_ID_LIGHTGUN_TURBO;
		const unsigned int DeviceIdLightgunPause = RETRO_DEVICE_ID_LIGHTGUN_PAUSE;
		const unsigned int DeviceIdLightgunStart = RETRO_DEVICE_ID_LIGHTGUN_START;
		const unsigned int DeviceIdPointerX = RETRO_DEVICE_ID_POINTER_X;
		const unsigned int DeviceIdPointerY = RETRO_DEVICE_ID_POINTER_Y;
		const unsigned int DeviceIdPointerPressed = RETRO_DEVICE_ID_POINTER_PRESSED;
	}
}