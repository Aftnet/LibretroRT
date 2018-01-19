namespace LibRetriX.RetroBindings.Tools
{
    public static class Converter
    {
        public static InputTypes ConvertToInputType(uint device, uint index, uint id)
        {
            switch (device)
            {
                case Constants.RETRO_DEVICE_JOYPAD:
                    switch (id)
                    {
                        case Constants.RETRO_DEVICE_ID_JOYPAD_B:
                            return InputTypes.DeviceIdJoypadB;
                        case Constants.RETRO_DEVICE_ID_JOYPAD_Y:
                            return InputTypes.DeviceIdJoypadY;
                        case Constants.RETRO_DEVICE_ID_JOYPAD_SELECT:
                            return InputTypes.DeviceIdJoypadSelect;
                        case Constants.RETRO_DEVICE_ID_JOYPAD_START:
                            return InputTypes.DeviceIdJoypadStart;
                        case Constants.RETRO_DEVICE_ID_JOYPAD_UP:
                            return InputTypes.DeviceIdJoypadUp;
                        case Constants.RETRO_DEVICE_ID_JOYPAD_DOWN:
                            return InputTypes.DeviceIdJoypadDown;
                        case Constants.RETRO_DEVICE_ID_JOYPAD_LEFT:
                            return InputTypes.DeviceIdJoypadLeft;
                        case Constants.RETRO_DEVICE_ID_JOYPAD_RIGHT:
                            return InputTypes.DeviceIdJoypadRight;
                        case Constants.RETRO_DEVICE_ID_JOYPAD_A:
                            return InputTypes.DeviceIdJoypadA;
                        case Constants.RETRO_DEVICE_ID_JOYPAD_X:
                            return InputTypes.DeviceIdJoypadX;
                        case Constants.RETRO_DEVICE_ID_JOYPAD_L:
                            return InputTypes.DeviceIdJoypadL;
                        case Constants.RETRO_DEVICE_ID_JOYPAD_R:
                            return InputTypes.DeviceIdJoypadR;
                        case Constants.RETRO_DEVICE_ID_JOYPAD_L2:
                            return InputTypes.DeviceIdJoypadL2;
                        case Constants.RETRO_DEVICE_ID_JOYPAD_R2:
                            return InputTypes.DeviceIdJoypadR2;
                        case Constants.RETRO_DEVICE_ID_JOYPAD_L3:
                            return InputTypes.DeviceIdJoypadL3;
                        case Constants.RETRO_DEVICE_ID_JOYPAD_R3:
                            return InputTypes.DeviceIdJoypadR3;
                        default:
                            return InputTypes.DeviceIdUnknown;
                    }
                case Constants.RETRO_DEVICE_ANALOG:
                    switch (index)
                    {
                        case Constants.RETRO_DEVICE_INDEX_ANALOG_LEFT:
                            switch (id)
                            {
                                case Constants.RETRO_DEVICE_ID_ANALOG_X:
                                    return InputTypes.DeviceIdAnalogLeftX;
                                case Constants.RETRO_DEVICE_ID_ANALOG_Y:
                                    return InputTypes.DeviceIdAnalogLeftY;
                                default:
                                    return InputTypes.DeviceIdUnknown;
                            }
                        case Constants.RETRO_DEVICE_INDEX_ANALOG_RIGHT:
                            switch (id)
                            {
                                case Constants.RETRO_DEVICE_ID_ANALOG_X:
                                    return InputTypes.DeviceIdAnalogRightX;
                                case Constants.RETRO_DEVICE_ID_ANALOG_Y:
                                    return InputTypes.DeviceIdAnalogRightY;
                                default:
                                    return InputTypes.DeviceIdUnknown;
                            }
                        default:
                            return InputTypes.DeviceIdUnknown;
                    }
                case Constants.RETRO_DEVICE_MOUSE:
                    switch (id)
                    {
                        case Constants.RETRO_DEVICE_ID_MOUSE_X:
                            return InputTypes.DeviceIdMouseX;
                        case Constants.RETRO_DEVICE_ID_MOUSE_Y:
                            return InputTypes.DeviceIdMouseY;
                        case Constants.RETRO_DEVICE_ID_MOUSE_LEFT:
                            return InputTypes.DeviceIdMouseLeft;
                        case Constants.RETRO_DEVICE_ID_MOUSE_RIGHT:
                            return InputTypes.DeviceIdMouseRight;
                        case Constants.RETRO_DEVICE_ID_MOUSE_WHEELUP:
                            return InputTypes.DeviceIdMouseWheelup;
                        case Constants.RETRO_DEVICE_ID_MOUSE_WHEELDOWN:
                            return InputTypes.DeviceIdMouseWheeldown;
                        case Constants.RETRO_DEVICE_ID_MOUSE_MIDDLE:
                            return InputTypes.DeviceIdMouseMiddle;
                        case Constants.RETRO_DEVICE_ID_MOUSE_HORIZ_WHEELUP:
                            return InputTypes.DeviceIdMouseHorizWheelup;
                        case Constants.RETRO_DEVICE_ID_MOUSE_HORIZ_WHEELDOWN:
                            return InputTypes.DeviceIdMouseHorizWheeldown;
                        default:
                            return InputTypes.DeviceIdUnknown;
                    }
                case Constants.RETRO_DEVICE_LIGHTGUN:
                    switch (id)
                    {
                        case Constants.RETRO_DEVICE_ID_LIGHTGUN_X:
                            return InputTypes.DeviceIdLightgunX;
                        case Constants.RETRO_DEVICE_ID_LIGHTGUN_Y:
                            return InputTypes.DeviceIdLightgunY;
                        case Constants.RETRO_DEVICE_ID_LIGHTGUN_TRIGGER:
                            return InputTypes.DeviceIdLightgunTrigger;
                        case Constants.RETRO_DEVICE_ID_LIGHTGUN_CURSOR:
                            return InputTypes.DeviceIdLightgunCursor;
                        case Constants.RETRO_DEVICE_ID_LIGHTGUN_TURBO:
                            return InputTypes.DeviceIdLightgunTurbo;
                        case Constants.RETRO_DEVICE_ID_LIGHTGUN_PAUSE:
                            return InputTypes.DeviceIdLightgunPause;
                        case Constants.RETRO_DEVICE_ID_LIGHTGUN_START:
                            return InputTypes.DeviceIdLightgunStart;
                        default:
                            return InputTypes.DeviceIdUnknown;
                    }
                case Constants.RETRO_DEVICE_POINTER:
                    switch (id)
                    {
                        case Constants.RETRO_DEVICE_ID_POINTER_X:
                            return InputTypes.DeviceIdPointerX;
                        case Constants.RETRO_DEVICE_ID_POINTER_Y:
                            return InputTypes.DeviceIdPointerY;
                        case Constants.RETRO_DEVICE_ID_POINTER_PRESSED:
                            return InputTypes.DeviceIdPointerPressed;
                        default:
                            return InputTypes.DeviceIdUnknown;
                    }
                default:
                    return InputTypes.DeviceIdUnknown;
            }
        }
    }
}
