using System;
using System.Runtime.InteropServices;

namespace LibRetriX.RetroBindings
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LibretroVariable
    {
        private IntPtr keyPtr;
        /// <summary>
        /// Variable to query in RETRO_ENVIRONMENT_GET_VARIABLE.
        /// If NULL, obtains the complete environment string if more complex parsing is necessary.
        /// The environment string is formatted as key-value pairs delimited by semicolons as so:
        /// "key1=value1;key2=value2;..."
        /// </summary>
        public IntPtr KeyPtr => keyPtr;

        private IntPtr valuePtr;
        /// <summary>
        /// Value to be obtained. If key does not exist, it is set to NULL.
        /// </summary>
        public IntPtr ValuePtr
        {
            get => valuePtr;
            set { valuePtr = value; }
        }
    };
}
