using LibretroRT;
using LibretroRT.AudioGraphPlayer;
using LibretroRT.InputManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Test
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public enum ConsoleType { GB, GBA, Genesis, NES, SNES };

        private static readonly IReadOnlyDictionary<PixelFormats, SurfaceFormat> PixelFormatToSurfaceMapping = new Dictionary<PixelFormats, SurfaceFormat>
        {
            { PixelFormats.Format0RGB1555, SurfaceFormat.Bgra5551 },
            { PixelFormats.FormatRGB565, SurfaceFormat.Bgr565 },
            { PixelFormats.FormatXRGB8888, SurfaceFormat.Color },
        };

        private static readonly IReadOnlyDictionary<PixelFormats, int> PixelFormatToSizeMapping = new Dictionary<PixelFormats, int>
        {
            { PixelFormats.Format0RGB1555, 2 },
            { PixelFormats.FormatRGB565, 2 },
            { PixelFormats.FormatXRGB8888, 4 },
        };

        private readonly IReadOnlyDictionary<ConsoleType, ICore> ConsoleTypeCoreMapping;

        readonly object CurrentCoreLock = new object();

        private ICore currentCore;
        private ICore CurrentCore
        {
            get { return currentCore; }
            set
            {
                if (currentCore == value)
                    return;

                if (currentCore != null)
                {
                    currentCore.RenderVideoFrame -= EmuCore_RenderVideoFrame;
                    currentCore.GameGeometryChanged -= EmuCore_GameGeometryChanged;
                    currentCore.PixelFormatChanged -= EmuCore_PixelFormatChanged;
                }

                currentCore = value;
                InputManager.Core = currentCore;
                MusicPlayer.Core = currentCore;

                if (currentCore == null)
                    return;

                currentCore.RenderVideoFrame += EmuCore_RenderVideoFrame;
                currentCore.GameGeometryChanged += EmuCore_GameGeometryChanged;
                currentCore.PixelFormatChanged += EmuCore_PixelFormatChanged;
            }
        }

        private IStorageFile CurrentRomFile;

        private Texture2D FrameBuffer;
        private SpriteFont font;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        uint frameNumber = 0;

        private readonly IAudioPlayer MusicPlayer = new AudioPlayer();
        private readonly IInputManager InputManager = new InputManager();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            ConsoleTypeCoreMapping = new Dictionary<ConsoleType, ICore>
            {
                { ConsoleType.GB, GambatteRT.GambatteCore.Instance },
                { ConsoleType.GBA, VBAMRT.VBAMCore.Instance },
                { ConsoleType.Genesis, GPGXRT.GPGXCore.Instance },
                { ConsoleType.SNES, Snes9XRT.Snes9XCore.Instance },
                { ConsoleType.NES, FCEUMMRT.FCEUMMCore.Instance },
            };
        }

        private void EmuCore_GameGeometryChanged(GameGeometry geometry)
        {
            lock (CurrentCoreLock)
            {
                UpdateFrameBuffer(geometry, CurrentCore.PixelFormat);
            }
        }

        private void EmuCore_PixelFormatChanged(PixelFormats format)
        {
            lock (CurrentCoreLock)
            {
                UpdateFrameBuffer(CurrentCore.Geometry, format);
            }
        }

        private void EmuCore_RenderVideoFrame(byte[] frameBuffer, uint width, uint height, uint pitch)
        {
            if (FrameBuffer == null)
                return;

            var targetWidth = pitch / PixelFormatToSizeMapping[CurrentCore.PixelFormat];
            var targetArea = new Rectangle(0, 0, (int)targetWidth, (int)height);
            FrameBuffer.SetData<byte>(0, targetArea, frameBuffer, 0, frameBuffer.Length);        
        }

        public string[] GetSupportedExtensions(ConsoleType consoleType)
        {
            return GetSupportedExtensions(ConsoleTypeCoreMapping[consoleType]);
        }

        public async void LoadRom(ConsoleType consoleType, IStorageFile storageFile)
        {
            CurrentRomFile = storageFile;

            MusicPlayer.Stop();
            await Task.Run(() =>
            {
                lock (CurrentCoreLock)
                {
                    CurrentCore?.UnloadGame();
                    CurrentCore = ConsoleTypeCoreMapping[consoleType];
                    CurrentCore.LoadGame(CurrentRomFile);
                    UpdateFrameBuffer(CurrentCore.Geometry, CurrentCore.PixelFormat);
                    MusicPlayer.ForceDetectSampleRate();
                }
            });
        }

        public async void SaveState()
        {
            lock (CurrentCoreLock)
            {
                if (CurrentCore == null)
                    return;
            }

            var file = await GetStateStorageFileAsync(true);
            if (file == null)
                return;

            using (var stream = (await file.OpenAsync(FileAccessMode.ReadWrite)).AsStream())
            {
                byte[] data;
                lock(CurrentCoreLock)
                {
                    data = new byte[CurrentCore.SerializationSize];
                    CurrentCore.Serialize(data);
                }
                var operation = stream.WriteAsync(data, 0, data.Length);
            }
        }

        public async void LoadState()
        {
            lock (CurrentCoreLock)
            {
                if (CurrentCore == null)
                    return;
            }

            var file = await GetStateStorageFileAsync(false);
            if (file == null)
                return;

            using (var stream = (await file.OpenAsync(FileAccessMode.Read)).AsStream())
            {
                var data = new byte[stream.Length];
                await stream.ReadAsync(data, 0, data.Length);
                lock (CurrentCoreLock)
                {
                    CurrentCore.Unserialize(data);
                }
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IsMouseVisible = true;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            font = Content.Load<SpriteFont>("Consolas");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            if(!MusicPlayer.ShouldDelayNextFrame)
            {
                lock (CurrentCoreLock)
                {
                    CurrentCore?.RunFrame();
                }
            }

            frameNumber++;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            var viewport = graphics.GraphicsDevice.Viewport;
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            if (FrameBuffer != null)
            {
                spriteBatch.Begin();
                lock (CurrentCoreLock)
                {
                    if (CurrentCore != null)
                    {
                        var viewportLocation = ComputeBestFittingSize(new Point(viewport.Width, viewport.Height), CurrentCore.Geometry.AspectRatio);
                        var frameBufferSize = new Point((int)CurrentCore.Geometry.BaseWidth, (int)CurrentCore.Geometry.BaseHeight);
                        spriteBatch.Draw(FrameBuffer, viewportLocation, new Rectangle(Point.Zero, frameBufferSize), Color.White);
                    }
                }
                //spriteBatch.DrawString(font, $"Frame {frameNumber}", new Vector2(0, 0), Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        private void UpdateFrameBuffer(GameGeometry geometry, PixelFormats pixelFormat)
        {
            var requestedSurfaceFormat = PixelFormatToSurfaceMapping[pixelFormat];

            int frameBufferSize = 2048;
            if (FrameBuffer == null || requestedSurfaceFormat != FrameBuffer.Format)
            {
                if (FrameBuffer != null)
                {
                    FrameBuffer.Dispose();
                }

                FrameBuffer = new Texture2D(graphics.GraphicsDevice, frameBufferSize, frameBufferSize, false, requestedSurfaceFormat);
            }
        }

        private static Rectangle ComputeBestFittingSize(Point viewportSize, float aspectRatio)
        {
            Rectangle output;
            int candidateWidth = (int)(viewportSize.Y * aspectRatio);
            if (viewportSize.X >= candidateWidth)
            {
                var size = new Point(candidateWidth, viewportSize.Y);
                output = new Rectangle(new Point((viewportSize.X - candidateWidth) / 2, 0), size);
            }
            else
            {
                var height = (int)(viewportSize.X / aspectRatio);
                var size = new Point(viewportSize.X, height);
                output = new Rectangle(new Point(0, (viewportSize.Y - height) / 2), size);
            }

            return output;
        }

        private async Task<StorageFile> GetStateStorageFileAsync(bool forSaving)
        {
            if (CurrentRomFile == null)
                return null;

            var folder = ApplicationData.Current.LocalFolder;
            var fileName = CurrentRomFile.Name + ".state";

            StorageFile file;
            if(forSaving)
            {
                file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            }
            else
            {
                file = (await folder.TryGetItemAsync(fileName)) as StorageFile;
            }

            return file;
        }

        private string[] GetSupportedExtensions(ICore core)
        {
            var extensions = core.SupportedExtensions;
            var output = extensions.Split('|').Select(d => "." + d).ToArray();
            return output;
        }
    }
}
