using System;
using System.Runtime.InteropServices;

namespace LibRetriX.RetroBindings
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ControllerDescription
    {
        /// <summary>
        /// Human-readable description of the controller.
        /// Even if using a generic input device type, this can be set to the particular device type the core uses.
        /// </summary>
        public IntPtr DescriptionStringPtr { get; private set; }

        /// <summary>
        /// Device type passed to retro_set_controller_port_device().
        /// If the device type is a sub-class of a generic input device type,
        /// use the RETRO_DEVICE_SUBCLASS macro to create an ID.
        /// E.g. RETRO_DEVICE_SUBCLASS(RETRO_DEVICE_JOYPAD, 1).
        /// </summary>
        public uint Id { get; private set; }
    };
}
