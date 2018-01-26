namespace LibRetriX.RetroBindings
{
    public static class Constants
    {
        public static int RETRO_DEVICE_SUBCLASS(int baseId, int id) { return ((id + 1) << RETRO_DEVICE_TYPE_SHIFT) | baseId; }

        /* Used for checking API/ABI mismatches that can break libretro
         * implementations.
         * It is not incremented for compatible changes to the API.
         */
        public const int RETRO_API_VERSION = 1;

        /*
         * Libretro's fundamental device abstractions.
         *
         * Libretro's input system consists of some standardized device types,
         * such as a joypad (with/without analog), mouse, keyboard, lightgun
         * and a pointer.
         *
         * The functionality of these devices are fixed, and individual cores
         * map their own concept of a controller to libretro's abstractions.
         * This makes it possible for frontends to map the abstract types to a
         * real input device, and not having to worry about binding input
         * correctly to arbitrary controller layouts.
         */

        public const int RETRO_DEVICE_TYPE_SHIFT = 8;
        public const int RETRO_DEVICE_MASK = ((1 << RETRO_DEVICE_TYPE_SHIFT) - 1);


        /* Input disabled. */
        public const int RETRO_DEVICE_NONE = 0;

        /* The JOYPAD is called RetroPad. It is essentially a Super Nintendo
         * controller, but with additional L2/R2/L3/R3 buttons, similar to a
         * PS1 DualShock. */
        public const int RETRO_DEVICE_JOYPAD = 1;

        /* The mouse is a simple mouse, similar to Super Nintendo's mouse.
         * X and Y coordinates are reported relatively to last poll (poll callback).
         * It is up to the libretro implementation to keep track of where the mouse
         * pointer is supposed to be on the screen.
         * The frontend must make sure not to interfere with its own hardware
         * mouse pointer.
         */
        public const int RETRO_DEVICE_MOUSE = 2;

        /* KEYBOARD device lets one poll for raw key pressed.
         * It is poll based, so input callback will return with the current
         * pressed state.
         * For event/text based keyboard input, see
         * RETRO_ENVIRONMENT_SET_KEYBOARD_CALLBACK.
         */
        public const int RETRO_DEVICE_KEYBOARD = 3;

        /* LIGHTGUN device is similar to Guncon-2 for PlayStation 2.
         * It reports X/Y coordinates in screen space (similar to the pointer)
         * in the range [-0x8000, 0x7fff] in both axes, with zero being center.
         * As well as reporting on/off screen state. It features a trigger,
         * start/select buttons, auxiliary action buttons and a
         * directional pad. A forced off-screen shot can be requested for
         * auto-reloading function in some games.
         */
        public const int RETRO_DEVICE_LIGHTGUN = 4;

        /* The ANALOG device is an extension to JOYPAD (RetroPad).
         * Similar to DualShock2 it adds two analog sticks and all buttons can
         * be analog. This is treated as a separate device type as it returns
         * axis values in the full analog range of [-0x8000, 0x7fff].
         * Positive X axis is right. Positive Y axis is down.
         * Buttons are returned in the range [0, 0x7fff].
         * Only use ANALOG type when polling for analog values.
         */
        public const int RETRO_DEVICE_ANALOG = 5;

        /* Abstracts the concept of a pointing mechanism, e.g. touch.
         * This allows libretro to query in absolute coordinates where on the
         * screen a mouse (or something similar) is being placed.
         * For a touch centric device, coordinates reported are the coordinates
         * of the press.
         *
         * Coordinates in X and Y are reported as:
         * [-0x7fff, 0x7fff]: -0x7fff corresponds to the far left/top of the screen,
         * and 0x7fff corresponds to the far right/bottom of the screen.
         * The "screen" is here defined as area that is passed to the frontend and
         * later displayed on the monitor.
         *
         * The frontend is free to scale/resize this screen as it sees fit, however,
         * (X, Y) = (-0x7fff, -0x7fff) will correspond to the top-left pixel of the
         * game image, etc.
         *
         * To check if the pointer coordinates are valid (e.g. a touch display
         * actually being touched), PRESSED returns 1 or 0.
         *
         * If using a mouse on a desktop, PRESSED will usually correspond to the
         * left mouse button, but this is a frontend decision.
         * PRESSED will only return 1 if the pointer is inside the game screen.
         *
         * For multi-touch, the index variable can be used to successively query
         * more presses.
         * If index = 0 returns true for _PRESSED, coordinates can be extracted
         * with _X, _Y for index = 0. One can then query _PRESSED, _X, _Y with
         * index = 1, and so on.
         * Eventually _PRESSED will return false for an index. No further presses
         * are registered at this point. */
        public const int RETRO_DEVICE_POINTER = 6;

        /* Buttons for the RetroPad (JOYPAD).
         * The placement of these is equivalent to placements on the
         * Super Nintendo controller.
         * L2/R2/L3/R3 buttons correspond to the PS1 DualShock.
         * Also used as id values for RETRO_DEVICE_INDEX_ANALOG_BUTTON */
        public const int RETRO_DEVICE_ID_JOYPAD_B = 0;
        public const int RETRO_DEVICE_ID_JOYPAD_Y = 1;
        public const int RETRO_DEVICE_ID_JOYPAD_SELECT = 2;
        public const int RETRO_DEVICE_ID_JOYPAD_START = 3;
        public const int RETRO_DEVICE_ID_JOYPAD_UP = 4;
        public const int RETRO_DEVICE_ID_JOYPAD_DOWN = 5;
        public const int RETRO_DEVICE_ID_JOYPAD_LEFT = 6;
        public const int RETRO_DEVICE_ID_JOYPAD_RIGHT = 7;
        public const int RETRO_DEVICE_ID_JOYPAD_A = 8;
        public const int RETRO_DEVICE_ID_JOYPAD_X = 9;
        public const int RETRO_DEVICE_ID_JOYPAD_L = 10;
        public const int RETRO_DEVICE_ID_JOYPAD_R = 11;
        public const int RETRO_DEVICE_ID_JOYPAD_L2 = 12;
        public const int RETRO_DEVICE_ID_JOYPAD_R2 = 13;
        public const int RETRO_DEVICE_ID_JOYPAD_L3 = 14;
        public const int RETRO_DEVICE_ID_JOYPAD_R3 = 15;

        /* Index / Id values for ANALOG device. */
        public const int RETRO_DEVICE_INDEX_ANALOG_LEFT = 0;
        public const int RETRO_DEVICE_INDEX_ANALOG_RIGHT = 1;
        public const int RETRO_DEVICE_INDEX_ANALOG_BUTTON = 2;
        public const int RETRO_DEVICE_ID_ANALOG_X = 0;
        public const int RETRO_DEVICE_ID_ANALOG_Y = 1;

        /* Id values for MOUSE. */
        public const int RETRO_DEVICE_ID_MOUSE_X = 0;
        public const int RETRO_DEVICE_ID_MOUSE_Y = 1;
        public const int RETRO_DEVICE_ID_MOUSE_LEFT = 2;
        public const int RETRO_DEVICE_ID_MOUSE_RIGHT = 3;
        public const int RETRO_DEVICE_ID_MOUSE_WHEELUP = 4;
        public const int RETRO_DEVICE_ID_MOUSE_WHEELDOWN = 5;
        public const int RETRO_DEVICE_ID_MOUSE_MIDDLE = 6;
        public const int RETRO_DEVICE_ID_MOUSE_HORIZ_WHEELUP = 7;
        public const int RETRO_DEVICE_ID_MOUSE_HORIZ_WHEELDOWN = 8;
        public const int RETRO_DEVICE_ID_MOUSE_BUTTON_4 = 9;
        public const int RETRO_DEVICE_ID_MOUSE_BUTTON_5 = 10;

        /* Id values for LIGHTGUN. */
        public const int RETRO_DEVICE_ID_LIGHTGUN_SCREEN_X = 13; /*Absolute Position*/
        public const int RETRO_DEVICE_ID_LIGHTGUN_SCREEN_Y = 14; /*Absolute*/
        public const int RETRO_DEVICE_ID_LIGHTGUN_IS_OFFSCREEN = 15; /*Status Check*/
        public const int RETRO_DEVICE_ID_LIGHTGUN_TRIGGER = 2;
        public const int RETRO_DEVICE_ID_LIGHTGUN_RELOAD = 16; /*Forced off-screen shot*/
        public const int RETRO_DEVICE_ID_LIGHTGUN_AUX_A = 3;
        public const int RETRO_DEVICE_ID_LIGHTGUN_AUX_B = 4;
        public const int RETRO_DEVICE_ID_LIGHTGUN_START = 6;
        public const int RETRO_DEVICE_ID_LIGHTGUN_SELECT = 7;
        public const int RETRO_DEVICE_ID_LIGHTGUN_AUX_C = 8;
        public const int RETRO_DEVICE_ID_LIGHTGUN_DPAD_UP = 9;
        public const int RETRO_DEVICE_ID_LIGHTGUN_DPAD_DOWN = 10;
        public const int RETRO_DEVICE_ID_LIGHTGUN_DPAD_LEFT = 11;
        public const int RETRO_DEVICE_ID_LIGHTGUN_DPAD_RIGHT = 12;
        /* deprecated */
        public const int RETRO_DEVICE_ID_LIGHTGUN_X = 0; /*Relative Position*/
        public const int RETRO_DEVICE_ID_LIGHTGUN_Y = 1; /*Relative*/
        public const int RETRO_DEVICE_ID_LIGHTGUN_CURSOR = 3; /*Use Aux:A*/
        public const int RETRO_DEVICE_ID_LIGHTGUN_TURBO = 4; /*Use Aux:B*/
        public const int RETRO_DEVICE_ID_LIGHTGUN_PAUSE = 5; /*Use Start*/

        /* Id values for POINTER. */
        public const int RETRO_DEVICE_ID_POINTER_X = 0;
        public const int RETRO_DEVICE_ID_POINTER_Y = 1;
        public const int RETRO_DEVICE_ID_POINTER_PRESSED = 2;

        /* Returned from retro_get_region(). */
        public const int RETRO_REGION_NTSC = 0;
        public const int RETRO_REGION_PAL = 1;


        /* Passed to retro_get_memory_data/size().
         * If the memory type doesn't apply to the
         * implementation NULL/0 can be returned.
         */
        public const int RETRO_MEMORY_MASK = 0xff;

        /* Regular save RAM. This RAM is usually found on a game cartridge,
         * backed up by a battery.
         * If save game data is too complex for a single memory buffer,
         * the SAVE_DIRECTORY (preferably) or SYSTEM_DIRECTORY environment
         * callback can be used. */
        public const int RETRO_MEMORY_SAVE_RAM = 0;

        /* Some games have a built-in clock to keep track of time.
         * This memory is usually just a couple of bytes to keep track of time.
         */
        public const int RETRO_MEMORY_RTC = 1;

        /* System ram lets a frontend peek into a game systems main RAM. */
        public const int RETRO_MEMORY_SYSTEM_RAM = 2;

        /* Video ram lets a frontend peek into a game systems video RAM (VRAM). */
        public const int RETRO_MEMORY_VIDEO_RAM = 3;

        /* If set, this call is not part of the public libretro API yet. It can
        * change or be removed at any time. */
        public const int RETRO_ENVIRONMENT_EXPERIMENTAL = 0x10000;
        /* Environment callback to be used internally in frontend. */
        public const int RETRO_ENVIRONMENT_PRIVATE = 0x20000;

        /* Environment commands. */
        public const int RETRO_ENVIRONMENT_SET_ROTATION = 1;  /* const unsigned * --
                                            * Sets screen rotation of graphics.
                                            * Is only implemented if rotation can be accelerated by hardware.
                                            * Valid values are 0, 1, 2, 3, which rotates screen by 0, 90, 180,
                                            * 270 degrees counter-clockwise respectively.
                                            */
        public const int RETRO_ENVIRONMENT_GET_OVERSCAN = 2;  /* bool * --
                                            * Boolean value whether or not the implementation should use overscan,
                                            * or crop away overscan.
                                            */
        public const int RETRO_ENVIRONMENT_GET_CAN_DUPE = 3;  /* bool * --
                                            * Boolean value whether or not frontend supports frame duping,
                                            * passing NULL to video frame callback.
                                            */

        /* Environ 4, 5 are no longer supported (GET_VARIABLE / SET_VARIABLES),
         * and reserved to avoid possible ABI clash.
         */

        public const int RETRO_ENVIRONMENT_SET_MESSAGE = 6;  /* const struct retro_message * --
                                            * Sets a message to be displayed in implementation-specific manner
                                            * for a certain amount of 'frames'.
                                            * Should not be used for trivial messages, which should simply be
                                            * logged via RETRO_ENVIRONMENT_GET_LOG_INTERFACE (or as a
                                            * fallback, stderr).
                                            */
        public const int RETRO_ENVIRONMENT_SHUTDOWN = 7;  /* N/A (NULL) --
                                            * Requests the frontend to shutdown.
                                            * Should only be used if game has a specific
                                            * way to shutdown the game from a menu item or similar.
                                            */
        public const int RETRO_ENVIRONMENT_SET_PERFORMANCE_LEVEL = 8;
        /* const unsigned * --
         * Gives a hint to the frontend how demanding this implementation
         * is on a system. E.g. reporting a level of 2 means
         * this implementation should run decently on all frontends
         * of level 2 and up.
         *
         * It can be used by the frontend to potentially warn
         * about too demanding implementations.
         *
         * The levels are "floating".
         *
         * This function can be called on a per-game basis,
         * as certain games an implementation can play might be
         * particularly demanding.
         * If called, it should be called in retro_load_game().
         */
        public const int RETRO_ENVIRONMENT_GET_SYSTEM_DIRECTORY = 9;
        /* const char ** --
         * Returns the "system" directory of the frontend.
         * This directory can be used to store system specific
         * content such as BIOSes, configuration data, etc.
         * The returned value can be NULL.
         * If so, no such directory is defined,
         * and it's up to the implementation to find a suitable directory.
         *
         * NOTE: Some cores used this folder also for "save" data such as
         * memory cards, etc, for lack of a better place to put it.
         * This is now discouraged, and if possible, cores should try to
         * use the new GET_SAVE_DIRECTORY.
         */
        public const int RETRO_ENVIRONMENT_SET_PIXEL_FORMAT = 10;
        /* const enum retro_pixel_format * --
         * Sets the internal pixel format used by the implementation.
         * The default pixel format is RETRO_PIXEL_FORMAT_0RGB1555.
         * This pixel format however, is deprecated (see enum retro_pixel_format).
         * If the call returns false, the frontend does not support this pixel
         * format.
         *
         * This function should be called inside retro_load_game() or
         * retro_get_system_av_info().
         */
        public const int RETRO_ENVIRONMENT_SET_INPUT_DESCRIPTORS = 11;
        /* const struct retro_input_descriptor * --
         * Sets an array of retro_input_descriptors.
         * It is up to the frontend to present this in a usable way.
         * The array is terminated by retro_input_descriptor::description
         * being set to NULL.
         * This function can be called at any time, but it is recommended
         * to call it as early as possible.
         */
        public const int RETRO_ENVIRONMENT_SET_KEYBOARD_CALLBACK = 12;
        /* const struct retro_keyboard_callback * --
         * Sets a callback function used to notify core about keyboard events.
         */
        public const int RETRO_ENVIRONMENT_SET_DISK_CONTROL_INTERFACE = 13;
        /* const struct retro_disk_control_callback * --
         * Sets an interface which frontend can use to eject and insert
         * disk images.
         * This is used for games which consist of multiple images and
         * must be manually swapped out by the user (e.g. PSX).
         */
        public const int RETRO_ENVIRONMENT_SET_HW_RENDER = 14;
        /* struct retro_hw_render_callback * --
         * Sets an interface to let a libretro core render with
         * hardware acceleration.
         * Should be called in retro_load_game().
         * If successful, libretro cores will be able to render to a
         * frontend-provided framebuffer.
         * The size of this framebuffer will be at least as large as
         * max_width/max_height provided in get_av_info().
         * If HW rendering is used, pass only RETRO_HW_FRAME_BUFFER_VALID or
         * NULL to retro_video_refresh_t.
         */
        public const int RETRO_ENVIRONMENT_GET_VARIABLE = 15;
        /* struct retro_variable * --
         * Interface to acquire user-defined information from environment
         * that cannot feasibly be supported in a multi-system way.
         * 'key' should be set to a key which has already been set by
         * SET_VARIABLES.
         * 'data' will be set to a value or NULL.
         */
        public const int RETRO_ENVIRONMENT_SET_VARIABLES = 16;
        /* const struct retro_variable * --
         * Allows an implementation to signal the environment
         * which variables it might want to check for later using
         * GET_VARIABLE.
         * This allows the frontend to present these variables to
         * a user dynamically.
         * This should be called as early as possible (ideally in
         * retro_set_environment).
         *
         * 'data' points to an array of retro_variable structs
         * terminated by a { NULL, NULL } element.
         * retro_variable::key should be namespaced to not collide
         * with other implementations' keys. E.g. A core called
         * 'foo' should use keys named as 'foo_option'.
         * retro_variable::value should contain a human readable
         * description of the key as well as a '|' delimited list
         * of expected values.
         *
         * The number of possible options should be very limited,
         * i.e. it should be feasible to cycle through options
         * without a keyboard.
         *
         * First entry should be treated as a default.
         *
         * Example entry:
         * { "foo_option", "Speed hack coprocessor X; false|true" }
         *
         * Text before first ';' is description. This ';' must be
         * followed by a space, and followed by a list of possible
         * values split up with '|'.
         *
         * Only strings are operated on. The possible values will
         * generally be displayed and stored as-is by the frontend.
         */
        public const int RETRO_ENVIRONMENT_GET_VARIABLE_UPDATE = 17;
        /* bool * --
         * Result is set to true if some variables are updated by
         * frontend since last call to RETRO_ENVIRONMENT_GET_VARIABLE.
         * Variables should be queried with GET_VARIABLE.
         */
        public const int RETRO_ENVIRONMENT_SET_SUPPORT_NO_GAME = 18;
        /* const bool * --
         * If true, the libretro implementation supports calls to
         * retro_load_game() with NULL as argument.
         * Used by cores which can run without particular game data.
         * This should be called within retro_set_environment() only.
         */
        public const int RETRO_ENVIRONMENT_GET_LIBRETRO_PATH = 19;
        /* const char ** --
         * Retrieves the absolute path from where this libretro
         * implementation was loaded.
         * NULL is returned if the libretro was loaded statically
         * (i.e. linked statically to frontend), or if the path cannot be
         * determined.
         * Mostly useful in cooperation with SET_SUPPORT_NO_GAME as assets can
         * be loaded without ugly hacks.
         */

        /* Environment 20 was an obsolete version of SET_AUDIO_CALLBACK.
         * It was not used by any known core at the time,
         * and was removed from the API. */
        public const int RETRO_ENVIRONMENT_SET_AUDIO_CALLBACK = 22;
        /* const struct retro_audio_callback * --
         * Sets an interface which is used to notify a libretro core about audio
         * being available for writing.
         * The callback can be called from any thread, so a core using this must
         * have a thread safe audio implementation.
         * It is intended for games where audio and video are completely
         * asynchronous and audio can be generated on the fly.
         * This interface is not recommended for use with emulators which have
         * highly synchronous audio.
         *
         * The callback only notifies about writability; the libretro core still
         * has to call the normal audio callbacks
         * to write audio. The audio callbacks must be called from within the
         * notification callback.
         * The amount of audio data to write is up to the implementation.
         * Generally, the audio callback will be called continously in a loop.
         *
         * Due to thread safety guarantees and lack of sync between audio and
         * video, a frontend  can selectively disallow this interface based on
         * internal configuration. A core using this interface must also
         * implement the "normal" audio interface.
         *
         * A libretro core using SET_AUDIO_CALLBACK should also make use of
         * SET_FRAME_TIME_CALLBACK.
         */
        public const int RETRO_ENVIRONMENT_SET_FRAME_TIME_CALLBACK = 21;
        /* const struct retro_frame_time_callback * --
         * Lets the core know how much time has passed since last
         * invocation of retro_run().
         * The frontend can tamper with the timing to fake fast-forward,
         * slow-motion, frame stepping, etc.
         * In this case the delta time will use the reference value
         * in frame_time_callback..
         */
        public const int RETRO_ENVIRONMENT_GET_RUMBLE_INTERFACE = 23;
        /* struct retro_rumble_interface * --
         * Gets an interface which is used by a libretro core to set
         * state of rumble motors in controllers.
         * A strong and weak motor is supported, and they can be
         * controlled indepedently.
         */
        public const int RETRO_ENVIRONMENT_GET_INPUT_DEVICE_CAPABILITIES = 24;
        /* uint64_t * --
         * Gets a bitmask telling which device type are expected to be
         * handled properly in a call to retro_input_state_t.
         * Devices which are not handled or recognized always return
         * 0 in retro_input_state_t.
         * Example bitmask: caps = (1 << RETRO_DEVICE_JOYPAD) | (1 << RETRO_DEVICE_ANALOG).
         * Should only be called in retro_run().
         */
        public const int RETRO_ENVIRONMENT_GET_SENSOR_INTERFACE = (25 | RETRO_ENVIRONMENT_EXPERIMENTAL);
        /* struct retro_sensor_interface * --
         * Gets access to the sensor interface.
         * The purpose of this interface is to allow
         * setting state related to sensors such as polling rate,
         * enabling/disable it entirely, etc.
         * Reading sensor state is done via the normal
         * input_state_callback API.
         */
        public const int RETRO_ENVIRONMENT_GET_CAMERA_INTERFACE = (26 | RETRO_ENVIRONMENT_EXPERIMENTAL);
        /* struct retro_camera_callback * --
         * Gets an interface to a video camera driver.
         * A libretro core can use this interface to get access to a
         * video camera.
         * New video frames are delivered in a callback in same
         * thread as retro_run().
         *
         * GET_CAMERA_INTERFACE should be called in retro_load_game().
         *
         * Depending on the camera implementation used, camera frames
         * will be delivered as a raw framebuffer,
         * or as an OpenGL texture directly.
         *
         * The core has to tell the frontend here which types of
         * buffers can be handled properly.
         * An OpenGL texture can only be handled when using a
         * libretro GL core (SET_HW_RENDER).
         * It is recommended to use a libretro GL core when
         * using camera interface.
         *
         * The camera is not started automatically. The retrieved start/stop
         * functions must be used to explicitly
         * start and stop the camera driver.
         */
        public const int RETRO_ENVIRONMENT_GET_LOG_INTERFACE = 27;
        /* struct retro_log_callback * --
         * Gets an interface for logging. This is useful for
         * logging in a cross-platform way
         * as certain platforms cannot use stderr for logging.
         * It also allows the frontend to
         * show logging information in a more suitable way.
         * If this interface is not used, libretro cores should
         * log to stderr as desired.
         */
        public const int RETRO_ENVIRONMENT_GET_PERF_INTERFACE = 28;
        /* struct retro_perf_callback * --
         * Gets an interface for performance counters. This is useful
         * for performance logging in a cross-platform way and for detecting
         * architecture-specific features, such as SIMD support.
         */
        public const int RETRO_ENVIRONMENT_GET_LOCATION_INTERFACE = 29;
        /* struct retro_location_callback * --
         * Gets access to the location interface.
         * The purpose of this interface is to be able to retrieve
         * location-based information from the host device,
         * such as current latitude / longitude.
         */
        public const int RETRO_ENVIRONMENT_GET_CONTENT_DIRECTORY = 30; /* Old name, kept for compatibility. */
        public const int RETRO_ENVIRONMENT_GET_CORE_ASSETS_DIRECTORY = 30;
        /* const char ** --
         * Returns the "core assets" directory of the frontend.
         * This directory can be used to store specific assets that the
         * core relies upon, such as art assets,
         * input data, etc etc.
         * The returned value can be NULL.
         * If so, no such directory is defined,
         * and it's up to the implementation to find a suitable directory.
         */
        public const int RETRO_ENVIRONMENT_GET_SAVE_DIRECTORY = 31;
        /* const char ** --
         * Returns the "save" directory of the frontend.
         * This directory can be used to store SRAM, memory cards,
         * high scores, etc, if the libretro core
         * cannot use the regular memory interface (retro_get_memory_data()).
         *
         * NOTE: libretro cores used to check GET_SYSTEM_DIRECTORY for
         * similar things before.
         * They should still check GET_SYSTEM_DIRECTORY if they want to
         * be backwards compatible.
         * The path here can be NULL. It should only be non-NULL if the
         * frontend user has set a specific save path.
         */
        public const int RETRO_ENVIRONMENT_SET_SYSTEM_AV_INFO = 32;
        /* const struct retro_system_av_info * --
         * Sets a new av_info structure. This can only be called from
         * within retro_run().
         * This should *only* be used if the core is completely altering the
         * internal resolutions, aspect ratios, timings, sampling rate, etc.
         * Calling this can require a full reinitialization of video/audio
         * drivers in the frontend,
         *
         * so it is important to call it very sparingly, and usually only with
         * the users explicit consent.
         * An eventual driver reinitialize will happen so that video and
         * audio callbacks
         * happening after this call within the same retro_run() call will
         * target the newly initialized driver.
         *
         * This callback makes it possible to support configurable resolutions
         * in games, which can be useful to
         * avoid setting the "worst case" in max_width/max_height.
         *
         * ***HIGHLY RECOMMENDED*** Do not call this callback every time
         * resolution changes in an emulator core if it's
         * expected to be a temporary change, for the reasons of possible
         * driver reinitialization.
         * This call is not a free pass for not trying to provide
         * correct values in retro_get_system_av_info(). If you need to change
         * things like aspect ratio or nominal width/height,
         * use RETRO_ENVIRONMENT_SET_GEOMETRY, which is a softer variant
         * of SET_SYSTEM_AV_INFO.
         *
         * If this returns false, the frontend does not acknowledge a
         * changed av_info struct.
         */
        public const int RETRO_ENVIRONMENT_SET_PROC_ADDRESS_CALLBACK = 33;
        /* const struct retro_get_proc_address_interface * --
         * Allows a libretro core to announce support for the
         * get_proc_address() interface.
         * This interface allows for a standard way to extend libretro where
         * use of environment calls are too indirect,
         * e.g. for cases where the frontend wants to call directly into the core.
         *
         * If a core wants to expose this interface, SET_PROC_ADDRESS_CALLBACK
         * **MUST** be called from within retro_set_environment().
         */
        public const int RETRO_ENVIRONMENT_SET_SUBSYSTEM_INFO = 34;
        /* const struct retro_subsystem_info * --
         * This environment call introduces the concept of libretro "subsystems".
         * A subsystem is a variant of a libretro core which supports
         * different kinds of games.
         * The purpose of this is to support e.g. emulators which might
         * have special needs, e.g. Super Nintendo's Super GameBoy, Sufami Turbo.
         * It can also be used to pick among subsystems in an explicit way
         * if the libretro implementation is a multi-system emulator itself.
         *
         * Loading a game via a subsystem is done with retro_load_game_special(),
         * and this environment call allows a libretro core to expose which
         * subsystems are supported for use with retro_load_game_special().
         * A core passes an array of retro_game_special_info which is terminated
         * with a zeroed out retro_game_special_info struct.
         *
         * If a core wants to use this functionality, SET_SUBSYSTEM_INFO
         * **MUST** be called from within retro_set_environment().
         */
        public const int RETRO_ENVIRONMENT_SET_CONTROLLER_INFO = 35;
        /* const struct retro_controller_info * --
         * This environment call lets a libretro core tell the frontend
         * which controller types are recognized in calls to
         * retro_set_controller_port_device().
         *
         * Some emulators such as Super Nintendo
         * support multiple lightgun types which must be specifically
         * selected from.
         * It is therefore sometimes necessary for a frontend to be able
         * to tell the core about a special kind of input device which is
         * not covered by the libretro input API.
         *
         * In order for a frontend to understand the workings of an input device,
         * it must be a specialized type
         * of the generic device types already defined in the libretro API.
         *
         * Which devices are supported can vary per input port.
         * The core must pass an array of const struct retro_controller_info which
         * is terminated with a blanked out struct. Each element of the struct
         * corresponds to an ascending port index to
         * retro_set_controller_port_device().
         * Even if special device types are set in the libretro core,
         * libretro should only poll input based on the base input device types.
         */
        public const int RETRO_ENVIRONMENT_SET_MEMORY_MAPS = (36 | RETRO_ENVIRONMENT_EXPERIMENTAL);
        /* const struct retro_memory_map * --
         * This environment call lets a libretro core tell the frontend
         * about the memory maps this core emulates.
         * This can be used to implement, for example, cheats in a core-agnostic way.
         *
         * Should only be used by emulators; it doesn't make much sense for
         * anything else.
         * It is recommended to expose all relevant pointers through
         * retro_get_memory_* as well.
         *
         * Can be called from retro_init and retro_load_game.
         */
        public const int RETRO_ENVIRONMENT_SET_GEOMETRY = 37;
        /* const struct retro_game_geometry * --
         * This environment call is similar to SET_SYSTEM_AV_INFO for changing
         * video parameters, but provides a guarantee that drivers will not be
         * reinitialized.
         * This can only be called from within retro_run().
         *
         * The purpose of this call is to allow a core to alter nominal
         * width/heights as well as aspect ratios on-the-fly, which can be
         * useful for some emulators to change in run-time.
         *
         * max_width/max_height arguments are ignored and cannot be changed
         * with this call as this could potentially require a reinitialization or a
         * non-constant time operation.
         * If max_width/max_height are to be changed, SET_SYSTEM_AV_INFO is required.
         *
         * A frontend must guarantee that this environment call completes in
         * constant time.
         */
        public const int RETRO_ENVIRONMENT_GET_USERNAME = 38;
        /* const char **
         * Returns the specified username of the frontend, if specified by the user.
         * This username can be used as a nickname for a core that has online facilities
         * or any other mode where personalization of the user is desirable.
         * The returned value can be NULL.
         * If this environ callback is used by a core that requires a valid username,
         * a default username should be specified by the core.
         */
        public const int RETRO_ENVIRONMENT_GET_LANGUAGE = 39;
        /* unsigned * --
         * Returns the specified language of the frontend, if specified by the user.
         * It can be used by the core for localization purposes.
         */
        public const int RETRO_ENVIRONMENT_GET_CURRENT_SOFTWARE_FRAMEBUFFER = (40 | RETRO_ENVIRONMENT_EXPERIMENTAL);
        /* struct retro_framebuffer * --
         * Returns a preallocated framebuffer which the core can use for rendering
         * the frame into when not using SET_HW_RENDER.
         * The framebuffer returned from this call must not be used
         * after the current call to retro_run() returns.
         *
         * The goal of this call is to allow zero-copy behavior where a core
         * can render directly into video memory, avoiding extra bandwidth cost by copying
         * memory from core to video memory.
         *
         * If this call succeeds and the core renders into it,
         * the framebuffer pointer and pitch can be passed to retro_video_refresh_t.
         * If the buffer from GET_CURRENT_SOFTWARE_FRAMEBUFFER is to be used,
         * the core must pass the exact
         * same pointer as returned by GET_CURRENT_SOFTWARE_FRAMEBUFFER;
         * i.e. passing a pointer which is offset from the
         * buffer is undefined. The width, height and pitch parameters
         * must also match exactly to the values obtained from GET_CURRENT_SOFTWARE_FRAMEBUFFER.
         *
         * It is possible for a frontend to return a different pixel format
         * than the one used in SET_PIXEL_FORMAT. This can happen if the frontend
         * needs to perform conversion.
         *
         * It is still valid for a core to render to a different buffer
         * even if GET_CURRENT_SOFTWARE_FRAMEBUFFER succeeds.
         *
         * A frontend must make sure that the pointer obtained from this function is
         * writeable (and readable).
         */

        public const int RETRO_ENVIRONMENT_SET_HW_SHARED_CONTEXT = (44 | RETRO_ENVIRONMENT_EXPERIMENTAL);
        /* N/A (null) * --
         * The frontend will try to use a 'shared' hardware context (mostly applicable
         * to OpenGL) when a hardware context is being set up.
         *
         * Returns true if the frontend supports shared hardware contexts and false
         * if the frontend does not support shared hardware contexts.
         *
         * This will do nothing on its own until SET_HW_RENDER env callbacks are
         * being used.
         */

        public const int RETRO_ENVIRONMENT_GET_VFS_INTERFACE = (45 | RETRO_ENVIRONMENT_EXPERIMENTAL);
        /* struct retro_vfs_interface_info * --
         * Gets access to the VFS interface.
         * VFS presence needs to be queried prior to load_game or any
         * get_system/save/other_directory being called to let front end know
         * core supports VFS before it starts handing out paths.
         * It is recomended to do so in retro_set_environment */


        /* File open flags
        * Introduced in VFS API v1 */
        public const int RETRO_VFS_FILE_ACCESS_READ = (1 << 0); /* Read only mode */
        public const int RETRO_VFS_FILE_ACCESS_WRITE = (1 << 1); /* Write only mode, discard contents and overwrites existing file unless RETRO_VFS_FILE_ACCESS_UPDATE is also specified */
        public const int RETRO_VFS_FILE_ACCESS_READ_WRITE = (RETRO_VFS_FILE_ACCESS_READ | RETRO_VFS_FILE_ACCESS_WRITE); /* Read-write mode, discard contents and overwrites existing file unless RETRO_VFS_FILE_ACCESS_UPDATE is also specified*/
        public const int RETRO_VFS_FILE_ACCESS_UPDATE_EXISTING = (1 << 2); /* Prevents discarding content of existing files opened for writing */

        /* These are only hints. The frontend may choose to ignore them. Other than RAM/CPU/etc use,
           and how they react to unlikely external interference (for example someone else writing to that file,
           or the file's server going down), behavior will not change. */
        public const int RETRO_VFS_FILE_ACCESS_HINT_NONE = (0);
        /* Indicate that the file will be accessed many times. The frontend should aggressively cache everything. */
        public const int RETRO_VFS_FILE_ACCESS_HINT_FREQUENT_ACCESS = (1 << 0);

        /* Seek positions */
        public const int RETRO_VFS_SEEK_POSITION_START = 0;
        public const int RETRO_VFS_SEEK_POSITION_CURRENT = 1;
        public const int RETRO_VFS_SEEK_POSITION_END = 2;

        public const int RETRO_ENVIRONMENT_GET_HW_RENDER_INTERFACE = (41 | RETRO_ENVIRONMENT_EXPERIMENTAL);
        /* const struct retro_hw_render_interface ** --
         * Returns an API specific rendering interface for accessing API specific data.
         * Not all HW rendering APIs support or need this.
         * The contents of the returned pointer is specific to the rendering API
         * being used. See the various headers like libretro_vulkan.h, etc.
         *
         * GET_HW_RENDER_INTERFACE cannot be called before context_reset has been called.
         * Similarly, after context_destroyed callback returns,
         * the contents of the HW_RENDER_INTERFACE are invalidated.
         */

        public const int RETRO_ENVIRONMENT_SET_SUPPORT_ACHIEVEMENTS = (42 | RETRO_ENVIRONMENT_EXPERIMENTAL);
        /* const bool * --
         * If true, the libretro implementation supports achievements
         * either via memory descriptors set with RETRO_ENVIRONMENT_SET_MEMORY_MAPS
         * or via retro_get_memory_data/retro_get_memory_size.
         *
         * This must be called before the first call to retro_run.
         */

        public const int RETRO_ENVIRONMENT_SET_HW_RENDER_CONTEXT_NEGOTIATION_INTERFACE = (43 | RETRO_ENVIRONMENT_EXPERIMENTAL);
        /* const struct retro_hw_render_context_negotiation_interface * --
         * Sets an interface which lets the libretro core negotiate with frontend how a context is created.
         * The semantics of this interface depends on which API is used in SET_HW_RENDER earlier.
         * This interface will be used when the frontend is trying to create a HW rendering context,
         * so it will be used after SET_HW_RENDER, but before the context_reset callback.
         */

        /* Serialized state is incomplete in some way. Set if serialization is
         * usable in typical end-user cases but should not be relied upon to
         * implement frame-sensitive frontend features such as netplay or
         * rerecording. */
        public const int RETRO_SERIALIZATION_QUIRK_INCOMPLETE = (1 << 0);
        /* The core must spend some time initializing before serialization is
         * supported. retro_serialize() will initially fail; retro_unserialize()
         * and retro_serialize_size() may or may not work correctly either. */
        public const int RETRO_SERIALIZATION_QUIRK_MUST_INITIALIZE = (1 << 1);
        /* Serialization size may change within a session. */
        public const int RETRO_SERIALIZATION_QUIRK_CORE_VARIABLE_SIZE = (1 << 2);
        /* Set by the frontend to acknowledge that it supports variable-sized
         * states. */
        public const int RETRO_SERIALIZATION_QUIRK_FRONT_VARIABLE_SIZE = (1 << 3);
        /* Serialized state can only be loaded during the same session. */
        public const int RETRO_SERIALIZATION_QUIRK_SINGLE_SESSION = (1 << 4);
        /* Serialized state cannot be loaded on an architecture with a different
         * endianness from the one it was saved on. */
        public const int RETRO_SERIALIZATION_QUIRK_ENDIAN_DEPENDENT = (1 << 5);
        /* Serialized state cannot be loaded on a different platform from the one it
         * was saved on for reasons other than endianness, such as word size
         * dependence */
        public const int RETRO_SERIALIZATION_QUIRK_PLATFORM_DEPENDENT = (1 << 6);

        public const int RETRO_ENVIRONMENT_SET_SERIALIZATION_QUIRKS = 44;
        /* uint64_t * --
         * Sets quirk flags associated with serialization. The frontend will zero any flags it doesn't
         * recognize or support. Should be set in either retro_init or retro_load_game, but not both.
         */

        public const int RETRO_MEMDESC_CONST = (1 << 0);   /* The frontend will never change this memory area once retro_load_game has returned. */
        public const int RETRO_MEMDESC_BIGENDIAN = (1 << 1);   /* The memory area contains big endian data. Default is little endian. */
        public const int RETRO_MEMDESC_ALIGN_2 = (1 << 16);  /* All memory access in this area is aligned to their own size, or 2, whichever is smaller. */
        public const int RETRO_MEMDESC_ALIGN_4 = (2 << 16);
        public const int RETRO_MEMDESC_ALIGN_8 = (3 << 16);
        public const int RETRO_MEMDESC_MINSIZE_2 = (1 << 24);  /* All memory in this region is accessed at least 2 bytes at the time. */
        public const int RETRO_MEMDESC_MINSIZE_4 = (2 << 24);
        public const int RETRO_MEMDESC_MINSIZE_8 = (3 << 24);

        /* Performance related functions */

        /* ID values for SIMD CPU features */
        public const int RETRO_SIMD_SSE = (1 << 0);
        public const int RETRO_SIMD_SSE2 = (1 << 1);
        public const int RETRO_SIMD_VMX = (1 << 2);
        public const int RETRO_SIMD_VMX128 = (1 << 3);
        public const int RETRO_SIMD_AVX = (1 << 4);
        public const int RETRO_SIMD_NEON = (1 << 5);
        public const int RETRO_SIMD_SSE3 = (1 << 6);
        public const int RETRO_SIMD_SSSE3 = (1 << 7);
        public const int RETRO_SIMD_MMX = (1 << 8);
        public const int RETRO_SIMD_MMXEXT = (1 << 9);
        public const int RETRO_SIMD_SSE4 = (1 << 10);
        public const int RETRO_SIMD_SSE42 = (1 << 11);
        public const int RETRO_SIMD_AVX2 = (1 << 12);
        public const int RETRO_SIMD_VFPU = (1 << 13);
        public const int RETRO_SIMD_PS = (1 << 14);
        public const int RETRO_SIMD_AES = (1 << 15);
        public const int RETRO_SIMD_VFPV3 = (1 << 16);
        public const int RETRO_SIMD_VFPV4 = (1 << 17);
        public const int RETRO_SIMD_POPCNT = (1 << 18);
        public const int RETRO_SIMD_MOVBE = (1 << 19);
        public const int RETRO_SIMD_CMOV = (1 << 20);
        public const int RETRO_SIMD_ASIMD = (1 << 21);

        public const int RETRO_MEMORY_ACCESS_WRITE = (1 << 0);
        /* The core will write to the buffer provided by retro_framebuffer::data. */
        public const int RETRO_MEMORY_ACCESS_READ = (1 << 1);
        /* The core will read from retro_framebuffer::data. */
        public const int RETRO_MEMORY_TYPE_CACHED = (1 << 0);
    }
}
