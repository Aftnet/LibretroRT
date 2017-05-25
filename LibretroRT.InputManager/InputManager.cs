using System.Collections.Generic;
using Windows.System;
using Windows.UI.Core;
using Windows.Gaming.Input;
using System.Linq;

namespace LibretroRT.InputManager
{
    public sealed class InputManager
    {
        private static readonly Dictionary<InputTypes, VirtualKey> LibretroGamepadToKeyboardKeyMapping = new Dictionary<InputTypes, VirtualKey>()
        {
            { InputTypes.DeviceIdJoypadUp, VirtualKey.Up },
            { InputTypes.DeviceIdJoypadDown, VirtualKey.Down },
            { InputTypes.DeviceIdJoypadLeft, VirtualKey.Left },
            { InputTypes.DeviceIdJoypadRight, VirtualKey.Right },
            { InputTypes.DeviceIdJoypadA, VirtualKey.A },
            { InputTypes.DeviceIdJoypadB, VirtualKey.S },
            { InputTypes.DeviceIdJoypadX, VirtualKey.Z },
            { InputTypes.DeviceIdJoypadY, VirtualKey.X },
            { InputTypes.DeviceIdJoypadSelect, VirtualKey.O },
            { InputTypes.DeviceIdJoypadStart, VirtualKey.P },
        };

        private static readonly Dictionary<InputTypes, GamepadButtons> LibretroGamepadToWindowsGamepadMapping = new Dictionary<InputTypes, GamepadButtons>()
        {
            { InputTypes.DeviceIdJoypadUp, GamepadButtons.DPadUp },
            { InputTypes.DeviceIdJoypadDown, GamepadButtons.DPadDown },
            { InputTypes.DeviceIdJoypadLeft, GamepadButtons.DPadLeft },
            { InputTypes.DeviceIdJoypadRight, GamepadButtons.DPadRight },
            { InputTypes.DeviceIdJoypadA, GamepadButtons.A },
            { InputTypes.DeviceIdJoypadB, GamepadButtons.B },
            { InputTypes.DeviceIdJoypadX, GamepadButtons.X },
            { InputTypes.DeviceIdJoypadY, GamepadButtons.Y },
            { InputTypes.DeviceIdJoypadSelect, GamepadButtons.View },
            { InputTypes.DeviceIdJoypadStart, GamepadButtons.Menu },
        };

        private readonly Dictionary<VirtualKey, CoreVirtualKeyStates> KeyStates = new Dictionary<VirtualKey, CoreVirtualKeyStates>();

        private GamepadReading[] GamepadReadings;

        public void PollInput()
        {
            var coreWindow = CoreWindow.GetForCurrentThread();
            foreach(var i in LibretroGamepadToKeyboardKeyMapping.Values)
            {
                KeyStates[i] = coreWindow.GetKeyState(i);
            }

            GamepadReadings = Gamepad.Gamepads.Select(d => d.GetCurrentReading()).ToArray();
        }

        public short GetInputState(uint port, InputTypes inputType)
        {
            var output = false;
            if (port == 0)
            {
                output = GetKeyboardKeyState(KeyStates, inputType);
            }

            if (port < GamepadReadings.Length)
            {
                output = output || GetGamepadButtonState(GamepadReadings[port], inputType);
            }

            return output ? (short)1 : (short)0;
        }

        private static bool GetKeyboardKeyState(Dictionary<VirtualKey, CoreVirtualKeyStates> keyStates, InputTypes button)
        {
            var nativeKey = LibretroGamepadToKeyboardKeyMapping[button];
            var output = keyStates[nativeKey] == CoreVirtualKeyStates.Down;
            return output;
        }

        private static bool GetGamepadButtonState(GamepadReading reading, InputTypes button)
        {
            var nativeButton = LibretroGamepadToWindowsGamepadMapping[button];
            var output = (reading.Buttons & nativeButton) == nativeButton;
            return output;
        }
    }
}
