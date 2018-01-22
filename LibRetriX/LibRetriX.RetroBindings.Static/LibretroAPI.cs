using System;
using System.Runtime.InteropServices;

namespace LibRetriX.RetroBindings
{
    internal static class LibretroAPI
    {
        #region Static callback delegates
        //This is to ensure native code has references to static members of a static class, which won't be moved by GC

        private static LibretroEnvironmentDelegate environmentCallback;
        public static LibretroEnvironmentDelegate EnvironmentCallback
        {
            get => environmentCallback;
            set { environmentCallback = value; SetEnvironmentDelegate(environmentCallback); }
        }

        private static LibretroRenderVideoFrameDelegate renderVideoFrameCallback;
        public static LibretroRenderVideoFrameDelegate RenderVideoFrameCallback
        {
            get => renderVideoFrameCallback;
            set { renderVideoFrameCallback = value; SetRenderVideoFrameDelegate(renderVideoFrameCallback); }
        }

        private static LibretroRenderAudioFrameDelegate renderAudioFrameCallback;
        public static LibretroRenderAudioFrameDelegate RenderAudioFrameCallback
        {
            get => renderAudioFrameCallback;
            set { renderAudioFrameCallback = value; SetRenderAudioFrameDelegate(renderAudioFrameCallback); }
        }

        private static LibretroRenderAudioFramesDelegate renderAudioFramesCallback;
        public static LibretroRenderAudioFramesDelegate RenderAudioFramesCallback
        {
            get => renderAudioFramesCallback;
            set { renderAudioFramesCallback = value; SetRenderAudioFramesDelegate(renderAudioFramesCallback); }
        }

        private static LibretroPollInputDelegate pollInputCallback;
        public static LibretroPollInputDelegate PollInputCallback
        {
            get => pollInputCallback;
            set { pollInputCallback = value; SetPollInputDelegate(pollInputCallback); }
        }

        private static LibretroGetInputStateDelegate getInputStateCallback;
        public static LibretroGetInputStateDelegate GetInputStateCallback
        {
            get => getInputStateCallback;
            set { getInputStateCallback = value; SetGetInputStateDelegate(getInputStateCallback); }
        }
        #endregion

        #region Internal callback setters
        [DllImport(NativeDllInfo.DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "retro_set_environment")]
        private extern static void SetEnvironmentDelegate(LibretroEnvironmentDelegate f);

        [DllImport(NativeDllInfo.DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "retro_set_video_refresh")]
        private extern static void SetRenderVideoFrameDelegate(LibretroRenderVideoFrameDelegate f);

        [DllImport(NativeDllInfo.DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "retro_set_audio_sample")]
        private extern static void SetRenderAudioFrameDelegate(LibretroRenderAudioFrameDelegate f);

        [DllImport(NativeDllInfo.DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "retro_set_audio_sample_batch")]
        private extern static void SetRenderAudioFramesDelegate(LibretroRenderAudioFramesDelegate f);

        [DllImport(NativeDllInfo.DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "retro_set_input_poll")]
        private extern static void SetPollInputDelegate(LibretroPollInputDelegate f);

        [DllImport(NativeDllInfo.DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "retro_set_input_state")]
        private extern static void SetGetInputStateDelegate(LibretroGetInputStateDelegate f);
        #endregion

        #region Libretro API methods
        /// <summary>
        /// Used to validate ABI compatibility when the API is revised.
        /// </summary>
        /// <returns>The current API version</returns>
        [DllImport(NativeDllInfo.DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "retro_api_version")]
        public extern static uint GetAPIVersion();

        /// <summary>
        ///  Gets statically known system info.
        ///  Pointers provided in *info must be statically allocated.
        ///  Can be called at any time, even before retro_init().
        /// </summary>
        /// <param name="info">Core system info</param>
        [DllImport(NativeDllInfo.DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "retro_get_system_info")]
        public extern static void GetSystemInfo(ref SystemInfo info);

        /// <summary>
        /// Gets information about system audio/video timings and geometry.
        /// Can be called only after retro_load_game() has successfully completed.
        /// The implementation of this function might not initialize every variable if needed.
        /// E.g.geom.aspect_ratio might not be initialized if core doesn't desire a particular aspect ratio.
        /// </summary>
        /// <param name="info">Core AV info</param>
        [DllImport(NativeDllInfo.DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "retro_get_system_av_info")]
        public extern static void GetSystemAvInfo(ref SystemAVInfo info);

        /// <summary>
        /// Gets region of game.
        /// </summary>
        /// <returns>The game region</returns>
        [DllImport(NativeDllInfo.DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "retro_get_region")]
        public extern static uint GetGameRegion();

        /// <summary>
        /// Core Initialization
        /// </summary>
        [DllImport(NativeDllInfo.DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "retro_init")]
        public extern static void Initialize();

        /// <summary>
        /// Core deinitialization
        /// </summary>
        [DllImport(NativeDllInfo.DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "retro_deinit")]
        public extern static void Cleanup();

        /// <summary>
        /// Sets device to be used for player 'port'.
        /// By default, RETRO_DEVICE_JOYPAD is assumed to be plugged into all available ports.
        /// Setting a particular device type is not a guarantee that libretro cores will only poll input based on that particular device type.
        /// It is only a hint to the libretro core when a core cannot automatically detect the appropriate input device type on its own.
        /// It is also relevant when a core can change its behavior depending on device type.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="device"></param>
        [DllImport(NativeDllInfo.DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "retro_set_controller_port_device")]
        public extern static void SetControllerPortDevice(uint port, uint device);

        /// <summary>
        /// Loads a game
        /// </summary>
        /// <param name="game">Game info struct</param>
        /// <returns>Whether the game was successfully loaded</returns>
        [DllImport(NativeDllInfo.DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "retro_load_game")]
        [return: MarshalAs(UnmanagedType.I1)]
        public extern static bool LoadGame([In] ref GameInfo game);

        /// <summary>
        /// Unloads a currently loaded game
        /// </summary>
        [DllImport(NativeDllInfo.DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "retro_unload_game")]
        public extern static void UnloadGame();

        /// <summary>
        /// Resets the current game
        /// </summary>
        [DllImport(NativeDllInfo.DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "retro_reset")]
        public extern static void Reset();

        /// <summary>
        /// Runs the game for one video frame.
        /// During retro_run(), input_poll callback must be called at least once.
        /// If a frame is not rendered for reasons where a game "dropped" a frame, this still counts as a frame, and retro_run() should explicitly dupe a frame if GET_CAN_DUPE returns true.
        /// In this case, the video callback can take a NULL argument for data.
        /// </summary>
        [DllImport(NativeDllInfo.DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "retro_run")]
        public extern static void RunFrame();

        /// <summary>
        /// Returns the amount of data the implementation requires to serialize internal state(save states).
        /// Between calls to retro_load_game() and retro_unload_game(), the returned size is never allowed to be larger than a previous returned value,
        /// to ensure that the frontend can allocate a save state buffer once.
        /// </summary>
        /// <returns>Size of serialization buffer</returns>
        [DllImport(NativeDllInfo.DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "retro_serialize_size")]
        public extern static IntPtr GetSerializationSize();

        [DllImport(NativeDllInfo.DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "retro_serialize")]
        [return: MarshalAs(UnmanagedType.I1)]
        public extern static bool SaveState(IntPtr data, IntPtr size);

        [DllImport(NativeDllInfo.DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "retro_unserialize")]
        [return: MarshalAs(UnmanagedType.I1)]
        public extern static bool LoadState(IntPtr data, IntPtr size);
        #endregion
    }
}
