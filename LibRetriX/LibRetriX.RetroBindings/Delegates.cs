using System;
using System.Runtime.InteropServices;

namespace LibRetriX.RetroBindings
{
    #region Main delegates
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LibretroLogDelegate(LogLevels level, IntPtr format, IntPtr argAddresses);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    public delegate bool LibretroEnvironmentDelegate(uint command, IntPtr data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LibretroRenderVideoFrameDelegate(IntPtr data, uint width, uint height, UIntPtr pitch);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LibretroRenderAudioFrameDelegate(short left, short right);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate UIntPtr LibretroRenderAudioFramesDelegate(IntPtr data,UIntPtr frames);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LibretroPollInputDelegate();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate short LibretroGetInputStateDelegate(uint port, uint device, uint index, uint id);
    #endregion

    #region VFS delgates
    /// <summary>
    /// Get path from opaque handle. Returns the exact same path passed to file_open when getting the handle
    /// Introduced in VFS API v1.
    /// </summary>
    /// <param name="stream">Opaque file handle</param>
    /// <returns>Path as C string</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr VFSGetPathDelegate(IntPtr stream);

    /// <summary>
    /// Open a file for reading or writing. If path points to a directory, this will fail.
    /// Introduced in VFS API v1.
    /// </summary>
    /// <param name="path">Requested path in virtual file system</param>
    /// <param name="mode">Access mode</param>
    /// <param name="hints">Ignore this</param>
    /// <returns>The opaque file handle, or NULL for error</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr VFSOpenDelegate(IntPtr path, uint mode, uint hints);

    /// <summary>
    /// Close the file and release its resources. Must be called if open_file returns non-NULL.
    /// Returns 0 on succes, -1 on failure.
    /// Whether the call succeeds ot not, the handle passed as parameter becomes invalid and should no longer be used.
    /// Introduced in VFS API v1.
    /// </summary>
    /// <param name="stream">Opaque file handle</param>
    /// <returns>0 on succes, -1 on failure</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int VFSCloseDelegate(IntPtr stream);

    /// <summary>
    /// Gets the file size.
    /// Introduced in VFS API v1.
    /// </summary>
    /// <param name="stream">Opaque file handle</param>
    /// <returns>Size of the file in bytes, or -1 for error</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long VFSGetSizeDelegate(IntPtr stream);

    /// <summary>
    /// Get the current read/write position for the file.
    /// Returns -1 for error.
    /// Introduced in VFS API v1
    /// </summary>
    /// <param name="stream">Opaque file handle</param>
    /// <returns>Current position or -1 for error</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long VFSGetPositionDelegate(IntPtr stream);

    /// <summary>
    /// Set the current read/write position for the file.
    /// Returns the new position, -1 for error.
    /// Introduced in VFS API v1.
    /// </summary>
    /// <param name="stream">Opaque file handle</param>
    /// <param name="offset">Offset</param>
    /// <param name="seekPosition">Position marker (start/current/end file)</param>
    /// <returns>New position in file, -1 for error</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long VFSSetPositionDelegate(IntPtr stream, long offset, int seekPosition);

    /// <summary>
    /// Read data from a file.
    /// Returns the number of bytes read, or -1 for error.
    /// Introduced in VFS API v1.
    /// </summary>
    /// <param name="stream">Opaque file handle</param>
    /// <param name="buffer">Destination buffer pointer</param>
    /// <param name="len">Num bytes to read</param>
    /// <returns>Number of bytes read, or -1 for error</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long VFSReadDelegate(IntPtr stream, IntPtr buffer, ulong len);

    /// <summary>
    /// Write data to a file.
    /// Returns the number of bytes written, or -1 for error.
    /// Introduced in VFS API v1.
    /// </summary>
    /// <param name="stream">Opaque file handle</param>
    /// <param name="buffer">Source buffer pointer</param>
    /// <param name="len">Num bytes to write</param>
    /// <returns>Number of bytes written, or -1 for error</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate long VFSWriteDelegate(IntPtr stream, IntPtr buffer, long len);

    /// <summary>
    /// Flush pending writes to file, if using buffered IO.
    /// Returns 0 on sucess, or -1 on failure.
    /// Introduced in VFS API v1.
    /// </summary>
    /// <param name="stream">Opaque file handle</param>
    /// <returns>0 on sucess, or -1 on failure</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int VFSFlushDelegate(IntPtr stream);

    /// <summary>
    /// Delete the specified file. Returns 0 on success, -1 on failure.
    /// Introduced in VFS API v1.
    /// </summary>
    /// <param name="path">Pointer to path string</param>
    /// <returns>0 on success, -1 on failure</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int VFSDeleteDelegate(IntPtr path);

    /// <summary>
    /// Rename the specified file. Returns 0 on success, -1 on failure.
    /// Introduced in VFS API v1.
    /// </summary>
    /// <param name="oldPath">Pointer to current path string</param>
    /// <param name="newPath">Pointer to desired new path string</param>
    /// <returns>0 on success, -1 on failure</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int VFSRenameDelegate(IntPtr oldPath, IntPtr newPath);
    #endregion
}
