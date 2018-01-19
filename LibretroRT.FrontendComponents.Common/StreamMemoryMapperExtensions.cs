using System.IO;
using System.Runtime.InteropServices;

namespace LibretroRT.FrontendComponents.Common
{
    public unsafe static class StreamMemoryMapperExtensions
    {
        public static bool TryGetPointer(this Stream input, out void* dataPtr, out GCHandle handle)
        {
            dataPtr = null;

            var memoryStream = input as MemoryStream;
            if (memoryStream != null)
            {
                var success = memoryStream.TryGetBuffer(out var buffer);
                if (!success)
                {
                    return false;
                }

                handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                dataPtr = handle.AddrOfPinnedObject().ToPointer();
                return true;
            }

            var unmanagedStream = input as UnmanagedMemoryStream;
            if (unmanagedStream != null)
            {
                dataPtr = unmanagedStream.PositionPointer;
                return true;
            }

            return false;
        }
    }
}
