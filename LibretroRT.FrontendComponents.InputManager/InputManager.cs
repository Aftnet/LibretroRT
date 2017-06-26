using LibretroRT.FrontendComponents.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Gaming.Input;
using Windows.System;
using Windows.UI.Core;

namespace LibretroRT.FrontendComponents.InputManager
{
    public sealed class InputManager : IInputManager
    {
        private const double GamepadAnalogDeadZoneSquareRadius = 0.0;

        private static readonly IReadOnlyDictionary<InputTypes, VirtualKey> LibretroGamepadToKeyboardKeyMapping = new Dictionary<InputTypes, VirtualKey>()
        {
            { InputTypes.DeviceIdJoypadUp, VirtualKey.Up },
            { InputTypes.DeviceIdJoypadDown, VirtualKey.Down },
            { InputTypes.DeviceIdJoypadLeft, VirtualKey.Left },
            { InputTypes.DeviceIdJoypadRight, VirtualKey.Right },
            { InputTypes.DeviceIdJoypadA, VirtualKey.A },
            { InputTypes.DeviceIdJoypadB, VirtualKey.S },
            { InputTypes.DeviceIdJoypadX, VirtualKey.Z },
            { InputTypes.DeviceIdJoypadY, VirtualKey.X },
            { InputTypes.DeviceIdJoypadL, VirtualKey.Q },
            { InputTypes.DeviceIdJoypadR, VirtualKey.W },
            { InputTypes.DeviceIdJoypadL2, VirtualKey.E },
            { InputTypes.DeviceIdJoypadR2, VirtualKey.R },
            { InputTypes.DeviceIdJoypadL3, VirtualKey.T },
            { InputTypes.DeviceIdJoypadR3, VirtualKey.Y },
            { InputTypes.DeviceIdJoypadSelect, VirtualKey.O },
            { InputTypes.DeviceIdJoypadStart, VirtualKey.P },
        };

        private static readonly IReadOnlyDictionary<InputTypes, Func<GamepadReading, short>> LibretroGamepadAnalogValueReadingsFunctionMapping = new Dictionary<InputTypes, Func<GamepadReading, short>>()
        {
            { InputTypes.DeviceIdAnalogLeftX, d => GetAnalogAxisValue(d.LeftThumbstickX, d.LeftThumbstickY) },
            { InputTypes.DeviceIdAnalogLeftY, d => GetAnalogAxisValue(d.LeftThumbstickY, d.LeftThumbstickX) },
            { InputTypes.DeviceIdAnalogRightX, d => GetAnalogAxisValue(d.RightThumbstickX, d.RightThumbstickY) },
            { InputTypes.DeviceIdAnalogRightY, d => GetAnalogAxisValue(d.RightThumbstickY, d.RightThumbstickX) },
        };

        private static readonly IReadOnlyDictionary<InputTypes, GamepadButtons> LibretroGamepadToWindowsGamepadButtonMapping = new Dictionary<InputTypes, GamepadButtons>()
        {
            { InputTypes.DeviceIdJoypadUp, GamepadButtons.DPadUp },
            { InputTypes.DeviceIdJoypadDown, GamepadButtons.DPadDown },
            { InputTypes.DeviceIdJoypadLeft, GamepadButtons.DPadLeft },
            { InputTypes.DeviceIdJoypadRight, GamepadButtons.DPadRight },
            { InputTypes.DeviceIdJoypadA, GamepadButtons.B },
            { InputTypes.DeviceIdJoypadB, GamepadButtons.A },
            { InputTypes.DeviceIdJoypadX, GamepadButtons.Y },
            { InputTypes.DeviceIdJoypadY, GamepadButtons.X },
            { InputTypes.DeviceIdJoypadL, GamepadButtons.LeftShoulder },
            { InputTypes.DeviceIdJoypadR, GamepadButtons.RightShoulder },
            { InputTypes.DeviceIdJoypadL2, GamepadButtons.Paddle1 },
            { InputTypes.DeviceIdJoypadR2, GamepadButtons.Paddle2 },
            { InputTypes.DeviceIdJoypadL3, GamepadButtons.LeftThumbstick },
            { InputTypes.DeviceIdJoypadR3, GamepadButtons.RightThumbstick },
            { InputTypes.DeviceIdJoypadSelect, GamepadButtons.View },
            { InputTypes.DeviceIdJoypadStart, GamepadButtons.Menu },
        };

        private readonly Dictionary<VirtualKey, bool> KeyStates = new Dictionary<VirtualKey, bool>();
        private readonly Dictionary<VirtualKey, bool> KeySnapshot = new Dictionary<VirtualKey, bool>();

        private GamepadReading[] GamepadReadings;

        public InputManager()
        {
            var window = CoreWindow.GetForCurrentThread();
            window.KeyDown -= WindowKeyDownHandler;
            window.KeyDown += WindowKeyDownHandler;
            window.KeyUp -= WindowKeyUpHandler;
            window.KeyUp += WindowKeyUpHandler;
        }

        public void PollInput()
        {
            foreach (var i in KeyStates.Keys)
            {
                KeySnapshot[i] = KeyStates[i];
            }

            GamepadReadings = Gamepad.Gamepads.Select(d => d.GetCurrentReading()).ToArray();
        }

        public short GetInputState(uint port, InputTypes inputType)
        {
            if (LibretroGamepadAnalogValueReadingsFunctionMapping.ContainsKey(inputType) && port < GamepadReadings.Length)
            {
                return LibretroGamepadAnalogValueReadingsFunctionMapping[inputType](GamepadReadings[port]);
            }

            var output = false;
            if (port == 0)
            {
                output = GetKeyboardKeyState(KeySnapshot, inputType);
            }

            if (port < GamepadReadings.Length)
            {
                output = output || GetGamepadButtonState(GamepadReadings[port], inputType);
            }

            return output ? (short)1 : (short)0;
        }

        private static bool GetKeyboardKeyState(Dictionary<VirtualKey, bool> keyStates, InputTypes button)
        {
            if (!LibretroGamepadToWindowsGamepadButtonMapping.ContainsKey(button))
            {
                return false;
            }

            var nativeKey = LibretroGamepadToKeyboardKeyMapping[button];
            var output = keyStates.ContainsKey(nativeKey) && keyStates[nativeKey];
            return output;
        }

        private static bool GetGamepadButtonState(GamepadReading reading, InputTypes button)
        {
            if (!LibretroGamepadToWindowsGamepadButtonMapping.ContainsKey(button))
            {
                return false;
            }

            var nativeButton = LibretroGamepadToWindowsGamepadButtonMapping[button];
            var output = (reading.Buttons & nativeButton) == nativeButton;
            return output;
        }

        private static short GetAnalogAxisValue(double axisValue, double perpendicularAxisValue)
        {
            if (axisValue * axisValue + perpendicularAxisValue * perpendicularAxisValue < GamepadAnalogDeadZoneSquareRadius)
            {
                return 0;
            }

            var scaledValue = axisValue * (double)short.MaxValue;
            return (short)scaledValue;
        }

        private void WindowKeyUpHandler(CoreWindow sender, KeyEventArgs args)
        {
            KeyStates[args.VirtualKey] = false;
        }

        private void WindowKeyDownHandler(CoreWindow sender, KeyEventArgs args)
        {
            KeyStates[args.VirtualKey] = true;
        }
    }
}
