using LibRetriX.RetroBindings.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace LibRetriX.RetroBindings
{
    internal sealed class LibretroCore : ICore, IDisposable
    {
        private readonly string name;
        public string Name => name;

        private readonly string version;
        public string Version => version;

        private readonly string[] supportedExtensions;
        public IReadOnlyList<string> SupportedExtensions => supportedExtensions;

        private readonly bool nativeArchiveSupport;
        public bool NativeArchiveSupport => nativeArchiveSupport;

        private readonly bool RequiresFullPath;

        private IntPtr currentlyResolvedCoreOptionValue;
        public IReadOnlyDictionary<string, CoreOption> Options { get; private set; }

        private IReadOnlyList<Tuple<string, uint>> OptionSetters { get; set; }

        private readonly IReadOnlyList<FileDependency> fileDependencies;
        public IReadOnlyList<FileDependency> FileDependencies => fileDependencies;

        private IntPtr systemRootPathUnmanaged;
        private string systemRootPath;
        public string SystemRootPath
        {
            get => systemRootPath;
            set { SetStringAndUnmanagedMemory(value, ref systemRootPath, ref systemRootPathUnmanaged); }
        }

        private IntPtr saveRootPathUnmanaged;
        private string saveRootPath;
        public string SaveRootPath
        {
            get => saveRootPath;
            set { SetStringAndUnmanagedMemory(value, ref saveRootPath, ref saveRootPathUnmanaged); }
        }

        private PixelFormats pixelFormat;
        public PixelFormats PixelFormat
        {
            get => pixelFormat;
            private set { pixelFormat = value; PixelFormatChanged?.Invoke(pixelFormat); }
        }

        private GameGeometry geometry;
        public GameGeometry Geometry
        {
            get => geometry;
            private set { geometry = value; GeometryChanged?.Invoke(geometry); }
        }

        private SystemTimings timings;
        public SystemTimings Timings
        {
            get => timings;
            private set { timings = value; TimingsChanged?.Invoke(timings); }
        }

        private Rotations rotation;
        public Rotations Rotation
        {
            get => rotation;
            private set { rotation = value; RotationChanged?.Invoke(rotation); }
        }

        public ulong SerializationSize => (ulong)LibretroAPI.GetSerializationSize();

        public PollInputDelegate PollInput { get; set; }
        public GetInputStateDelegate GetInputState { get; set; }
        public OpenFileStreamDelegate OpenFileStream
        {
            get => VFSHandler.OpenFileStream;
            set { VFSHandler.OpenFileStream = value; }
        }

        public CloseFileStreamDelegate CloseFileStream
        {
            get => VFSHandler.CloseFileStream;
            set { VFSHandler.CloseFileStream = value; }
        }

        public event RenderVideoFrameDelegate RenderVideoFrame;
        public event RenderAudioFramesDelegate RenderAudioFrames;
        public event PixelFormatChangedDelegate PixelFormatChanged;
        public event GeometryChangedDelegate GeometryChanged;
        public event TimingsChangedDelegate TimingsChanged;
        public event RotationChangedDelegate RotationChanged;

        private static readonly LogCallbackDescriptor LogCBDescriptor = new LogCallbackDescriptor { LogCallback = LogHandler };

        private readonly uint inputTypeIndex;
        private uint InputTypeIndex => inputTypeIndex;

        private uint InputTypeId { get; set; }

        private bool IsInitialized { get; set; }
        private GameInfo? CurrentGameInfo { get; set; }
        private GCHandle GameDataHandle { get; set; }

        public LibretroCore(IReadOnlyList<FileDependency> dependencies = null, IReadOnlyList<Tuple<string, uint>> optionSetters = null, uint inputTypeIx = 0)
        {
            fileDependencies = dependencies == null ? Array.Empty<FileDependency>() : dependencies;
            OptionSetters = optionSetters == null ? Array.Empty<Tuple<string, uint>>() : optionSetters;
            inputTypeIndex = inputTypeIx;

            LibretroAPI.EnvironmentCallback = EnvironmentHandler;
            LibretroAPI.RenderVideoFrameCallback = RenderVideoFrameHandler;
            LibretroAPI.RenderAudioFrameCallback = RenderAudioFrameHandler;
            LibretroAPI.RenderAudioFramesCallback = RenderAudioFramesHandler;
            LibretroAPI.PollInputCallback = PollInputHandler;
            LibretroAPI.GetInputStateCallback = GetInputStateHandler;

            var systemInfo = new SystemInfo();
            LibretroAPI.GetSystemInfo(ref systemInfo);
            name = systemInfo.LibraryName;
            version = systemInfo.LibraryVersion;
            supportedExtensions = systemInfo.ValidExtensions.Split('|').Select(d => $".{d}").ToArray();
            nativeArchiveSupport = systemInfo.BlockExtract;
            RequiresFullPath = systemInfo.NeedFullpath;

            Options = new Dictionary<string, CoreOption>();
        }

        public void Dispose()
        {
            SystemRootPath = null;
            SaveRootPath = null;

            if (currentlyResolvedCoreOptionValue != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(currentlyResolvedCoreOptionValue);
                currentlyResolvedCoreOptionValue = IntPtr.Zero;
            }
        }

        public bool LoadGame(string mainGameFilePath)
        {
            if (!IsInitialized)
            {
                LibretroAPI.Initialize();
                IsInitialized = true;
            }

            if (CurrentGameInfo.HasValue)
            {
                UnloadGameNoDeinit();
            }

            var gameInfo = new GameInfo()
            {
                Path = mainGameFilePath
            };

            if (!RequiresFullPath)
            {
                var stream = OpenFileStream?.Invoke(mainGameFilePath, FileAccess.Read);
                if (stream == null)
                {
                    return false;
                }

                var data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                GameDataHandle = gameInfo.SetGameData(data);
                CloseFileStream(stream);
            }

            Rotation = Rotations.CCW0;

            var loadSuccessful = LibretroAPI.LoadGame(ref gameInfo);
            if (loadSuccessful)
            {
                var avInfo = new SystemAVInfo();
                LibretroAPI.GetSystemAvInfo(ref avInfo);

                Geometry = avInfo.Geometry;
                Timings = avInfo.Timings;

                LibretroAPI.SetControllerPortDevice(0, InputTypeId);
                CurrentGameInfo = gameInfo;
            }

            return CurrentGameInfo.HasValue;
        }

        public void UnloadGame()
        {
            UnloadGameNoDeinit();

            if (IsInitialized)
            {
                LibretroAPI.Cleanup();
                IsInitialized = false;
            }
        }

        public void Reset()
        {
            LibretroAPI.Reset();
        }

        public void RunFrame()
        {
            LibretroAPI.RunFrame();
        }

        public bool SaveState(Stream outputStream)
        {
            var size = LibretroAPI.GetSerializationSize();
            var stateData = new byte[(int)size];

            var handle = GCHandle.Alloc(stateData, GCHandleType.Pinned);
            var result = LibretroAPI.SaveState(handle.AddrOfPinnedObject(), (IntPtr)stateData.Length);
            handle.Free();

            if (result == true)
            {
                outputStream.Position = 0;
                outputStream.Write(stateData, 0, stateData.Length);
                outputStream.SetLength(stateData.Length);
            }

            return result;
        }

        public bool LoadState(Stream inputStream)
        {
            var stateData = new byte[inputStream.Length];
            inputStream.Position = 0;
            inputStream.Read(stateData, 0, stateData.Length);

            var handle = GCHandle.Alloc(stateData, GCHandleType.Pinned);
            var result = LibretroAPI.LoadState(handle.AddrOfPinnedObject(), (IntPtr)stateData.Length);
            handle.Free();

            return result;
        }

        private void UnloadGameNoDeinit()
        {
            if (!CurrentGameInfo.HasValue)
            {
                return;
            }

            LibretroAPI.UnloadGame();
            if (GameDataHandle.IsAllocated)
            {
                GameDataHandle.Free();
            }

            CurrentGameInfo = null;
        }

        private bool EnvironmentHandler(uint command, IntPtr dataPtr)
        {
            switch (command)
            {
                case Constants.RETRO_ENVIRONMENT_GET_LOG_INTERFACE:
                    {
                        Marshal.StructureToPtr(LogCBDescriptor, dataPtr, false);
                        return true;
                    }
                case Constants.RETRO_ENVIRONMENT_SET_VARIABLES:
                    {
                        var newOptions = new Dictionary<string, CoreOption>();
                        Options = newOptions;

                        var data = Marshal.PtrToStructure<LibretroVariable>(dataPtr);
                        while (data.KeyPtr != IntPtr.Zero)
                        {
                            var key = Marshal.PtrToStringAnsi(data.KeyPtr);
                            var rawValue = Marshal.PtrToStringAnsi(data.ValuePtr);

                            var split = rawValue.Split(';');
                            var description = split[0];

                            rawValue = rawValue.Substring(description.Length + 2);
                            split = rawValue.Split('|');

                            newOptions.Add(key, new CoreOption(description, split));

                            dataPtr += Marshal.SizeOf<LibretroVariable>();
                            data = Marshal.PtrToStructure<LibretroVariable>(dataPtr);
                        }

                        foreach(var i in OptionSetters)
                        {
                            Options[i.Item1].SelectedValueIx = i.Item2;
                        }

                        return true;
                    }
                case Constants.RETRO_ENVIRONMENT_GET_VARIABLE:
                    {
                        var data = Marshal.PtrToStructure<LibretroVariable>(dataPtr);
                        var key = Marshal.PtrToStringAnsi(data.KeyPtr);
                        var valueFound = false;
                        data.ValuePtr = IntPtr.Zero;

                        if (Options.ContainsKey(key))
                        {
                            valueFound = true;
                            var coreOption = Options[key];
                            var value = coreOption.Values[(int)coreOption.SelectedValueIx];
                            if (currentlyResolvedCoreOptionValue != IntPtr.Zero)
                            {
                                Marshal.FreeHGlobal(currentlyResolvedCoreOptionValue);
                            }

                            currentlyResolvedCoreOptionValue = Marshal.StringToHGlobalAnsi(value);
                            data.ValuePtr = currentlyResolvedCoreOptionValue;
                        }

                        Marshal.StructureToPtr(data, dataPtr, false);
                        return valueFound;
                    }
                case Constants.RETRO_ENVIRONMENT_GET_OVERSCAN:
                    {
                        Marshal.WriteByte(dataPtr, 0);
                        return true;
                    }
                case Constants.RETRO_ENVIRONMENT_GET_CAN_DUPE:
                    {
                        Marshal.WriteByte(dataPtr, 1);
                        return true;
                    }
                case Constants.RETRO_ENVIRONMENT_GET_SYSTEM_DIRECTORY:
                    {
                        Marshal.WriteIntPtr(dataPtr, systemRootPathUnmanaged);
                        return true;
                    }
                case Constants.RETRO_ENVIRONMENT_GET_SAVE_DIRECTORY:
                    {
                        Marshal.WriteIntPtr(dataPtr, saveRootPathUnmanaged);
                        return true;
                    }
                case Constants.RETRO_ENVIRONMENT_SET_PIXEL_FORMAT:
                    {
                        var data = (PixelFormats)Marshal.ReadInt32(dataPtr);
                        PixelFormat = data;
                        return true;
                    }
                case Constants.RETRO_ENVIRONMENT_SET_GEOMETRY:
                    {
                        var data = Marshal.PtrToStructure<GameGeometry>(dataPtr);
                        Geometry = data;
                        return true;
                    }
                case Constants.RETRO_ENVIRONMENT_SET_ROTATION:
                    {
                        var data = (Rotations)Marshal.ReadInt32(dataPtr);
                        Rotation = data;
                        return true;
                    }
                case Constants.RETRO_ENVIRONMENT_SET_SYSTEM_AV_INFO:
                    {
                        var data = Marshal.PtrToStructure<SystemAVInfo>(dataPtr);
                        Geometry = data.Geometry;
                        Timings = data.Timings;
                        return true;
                    }
                case Constants.RETRO_ENVIRONMENT_SET_CONTROLLER_INFO:
                    {
                        var data = Marshal.PtrToStructure<ControllerInfo>(dataPtr);
                        if (InputTypeIndex < data.NumDescriptions)
                        {
                            var descriptionPtr = data.DescriptionsPtr + ((int)InputTypeIndex * Marshal.SizeOf<ControllerDescription>());
                            var description = Marshal.PtrToStructure<ControllerDescription>(descriptionPtr);
                            InputTypeId = description.Id;
                        }

                        return true;
                    }
                case Constants.RETRO_ENVIRONMENT_GET_VFS_INTERFACE:
                    {
                        var data = Marshal.PtrToStructure<VFSInterfaceInfo>(dataPtr);
                        if (data.RequiredInterfaceVersion <= VFSHandler.SupportedVFSVersion)
                        {
                            data.RequiredInterfaceVersion = VFSHandler.SupportedVFSVersion;
                            data.Interface = VFSHandler.VFSInterfacePtr;
                            Marshal.StructureToPtr(data, dataPtr, false);
                        }

                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        private static void LogHandler(LogLevels level, IntPtr format, IntPtr argAddresses)
        {
#if DEBUG
            var message = Marshal.PtrToStringAnsi(format);
            System.Diagnostics.Debug.WriteLine($"{NativeDllInfo.DllName}: {level} - {message}");
#endif
        }

        private void RenderVideoFrameHandler(IntPtr data, uint width, uint height, IntPtr pitch)
        {
            //Duped frame
            if (data == IntPtr.Zero)
            {
                RenderVideoFrame?.Invoke(null, width, height, (ulong)pitch);
                return;
            }

            unsafe
            {
                using (var stream = new UnmanagedMemoryStream((byte*)data.ToPointer(), height * (long)pitch))
                {
                    RenderVideoFrame?.Invoke(stream, width, height, (ulong)pitch);
                }
            }
        }

        private void RenderAudioFrameHandler(short left, short right)
        {
            var data = BitConverter.GetBytes(left).Concat(BitConverter.GetBytes(right)).ToArray();
            using (var stream = new MemoryStream(data))
            {
                RenderAudioFrames?.Invoke(stream, 1);
            }
        }

        private IntPtr RenderAudioFramesHandler(IntPtr data, IntPtr numFrames)
        {
            unsafe
            {
                using (var stream = new UnmanagedMemoryStream((byte*)data.ToPointer(), (long)numFrames * Marshal.SizeOf<short>() * 2))
                {
                    RenderAudioFrames?.Invoke(stream, (ulong)numFrames);
                }
            }

            return IntPtr.Zero;
        }

        private void PollInputHandler()
        {
            PollInput?.Invoke();
        }

        private short GetInputStateHandler(uint port, uint device, uint index, uint id)
        {
            var inputType = Converter.ConvertToInputType(device, index, id);
            var result = GetInputState?.Invoke(port, inputType);
            return result ?? 0;
        }

        private void SetStringAndUnmanagedMemory(string newValue, ref string store, ref IntPtr unmanagedPtr)
        {
            store = newValue;
            if (unmanagedPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(unmanagedPtr);
                unmanagedPtr = IntPtr.Zero;
            }

            if (newValue != null)
            {
                unmanagedPtr = Marshal.StringToHGlobalAnsi(newValue);
            }
        }
    }
}
