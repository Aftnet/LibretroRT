#pragma once

namespace LibretroRT
{
	public enum class PixelFormats
	{
		Format0RGB1555,
		FormatXRGB8888,
		FormatRGB565,
		FormatUknown,
	};

	public enum class Rotations
	{
		CCW0 = 0,
		CCW90 = 1,
		CCW180 = 2,
		CCW270 = 3,
	};

	public enum class InputTypes
	{
		DeviceIdJoypadB,
		DeviceIdJoypadY,
		DeviceIdJoypadSelect,
		DeviceIdJoypadStart,
		DeviceIdJoypadUp,
		DeviceIdJoypadDown,
		DeviceIdJoypadLeft,
		DeviceIdJoypadRight,
		DeviceIdJoypadA,
		DeviceIdJoypadX,
		DeviceIdJoypadL,
		DeviceIdJoypadR,
		DeviceIdJoypadL2,
		DeviceIdJoypadR2,
		DeviceIdJoypadL3,
		DeviceIdJoypadR3,
		DeviceIdAnalogLeftX,
		DeviceIdAnalogLeftY,
		DeviceIdAnalogRightX,
		DeviceIdAnalogRightY,
		DeviceIdMouseX,
		DeviceIdMouseY,
		DeviceIdMouseLeft,
		DeviceIdMouseRight,
		DeviceIdMouseWheelup,
		DeviceIdMouseWheeldown,
		DeviceIdMouseMiddle,
		DeviceIdMouseHorizWheelup,
		DeviceIdMouseHorizWheeldown,
		DeviceIdLightgunX,
		DeviceIdLightgunY,
		DeviceIdLightgunTrigger,
		DeviceIdLightgunCursor,
		DeviceIdLightgunTurbo,
		DeviceIdLightgunPause,
		DeviceIdLightgunStart,
		DeviceIdPointerX,
		DeviceIdPointerY,
		DeviceIdPointerPressed,
		DeviceIdUnknown
	};
}