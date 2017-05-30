using System.Collections.Generic;
using Windows.System;
using Windows.UI.Core;
using Windows.Gaming.Input;
using System.Linq;
using System;

namespace LibretroRT.InputManager
{
    public sealed class InputManager : IInputManager
    {
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
            { InputTypes.DeviceIdJoypadSelect, VirtualKey.O },
            { InputTypes.DeviceIdJoypadStart, VirtualKey.P },
        };

        private static readonly IReadOnlyDictionary<InputTypes, GamepadButtons> LibretroGamepadToWindowsGamepadMapping = new Dictionary<InputTypes, GamepadButtons>()
        {
            { InputTypes.DeviceIdJoypadUp, GamepadButtons.DPadUp },
            { InputTypes.DeviceIdJoypadDown, GamepadButtons.DPadDown },
            { InputTypes.DeviceIdJoypadLeft, GamepadButtons.DPadLeft },
            { InputTypes.DeviceIdJoypadRight, GamepadButtons.DPadRight },
            { InputTypes.DeviceIdJoypadA, GamepadButtons.A },
            { InputTypes.DeviceIdJoypadB, GamepadButtons.B },
            { InputTypes.DeviceIdJoypadX, GamepadButtons.X },
            { InputTypes.DeviceIdJoypadY, GamepadButtons.Y },
            { InputTypes.DeviceIdJoypadL, GamepadButtons.LeftShoulder },
            { InputTypes.DeviceIdJoypadR, GamepadButtons.RightShoulder },
            { InputTypes.DeviceIdJoypadL2, GamepadButtons.Paddle1 },
            { InputTypes.DeviceIdJoypadR2, GamepadButtons.Paddle2 },
            { InputTypes.DeviceIdJoypadSelect, GamepadButtons.View },
            { InputTypes.DeviceIdJoypadStart, GamepadButtons.Menu },
        };

        private readonly Lazy<CoreWindow> Window;

        private readonly Dictionary<VirtualKey, bool> KeyStates = new Dictionary<VirtualKey, bool>();
        private readonly Dictionary<VirtualKey, bool> KeySnapshot = new Dictionary<VirtualKey, bool>();

        private GamepadReading[] GamepadReadings;

        public InputManager()
        {
            Window = new Lazy<CoreWindow>(() =>
             {
                 var window = CoreWindow.GetForCurrentThread();
                 window.KeyDown += WindowKeyDownHandler;
                 window.KeyUp += WindowKeyUpHandler;
                 return window;
             });
        }

        public void PollInput()
        {
            var window = Window.Value;
            foreach (var i in KeyStates.Keys)
            {
                KeySnapshot[i] = KeyStates[i];
            }

            GamepadReadings = Gamepad.Gamepads.Select(d => d.GetCurrentReading()).ToArray();
        }

        public short GetInputState(uint port, InputTypes inputType)
        {
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
            var nativeKey = LibretroGamepadToKeyboardKeyMapping[button];
            var output = keyStates.ContainsKey(nativeKey) && keyStates[nativeKey];
            return output;
        }

        private static bool GetGamepadButtonState(GamepadReading reading, InputTypes button)
        {
            var nativeButton = LibretroGamepadToWindowsGamepadMapping[button];
            var output = (reading.Buttons & nativeButton) == nativeButton;
            return output;
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
