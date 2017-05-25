using LibretroRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading.Tasks;
using Windows.Storage;

namespace Test
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private bool CoreShouldRun = false;
        private readonly ICore EmuCore = GPGXRT.GPGXCore.Instance;

        private Texture2D FrameBuffer;
        private SpriteFont font;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        uint frameNumber = 0;

        private readonly AudioPlayer MusicPlayer = new AudioPlayer();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            EmuCore.RenderVideoFrame += EmuCore_RenderVideoFrame;
            EmuCore.RenderAudioFrames += EmuCore_RenderAudioFrames;
            EmuCore.PollInput += EmuCore_PollInput;
            EmuCore.GetInputState += EmuCore_GetInputState;
            EmuCore.GameGeometryChanged += EmuCore_GameGeometryChanged;
            EmuCore.SystemTimingChanged += EmuCore_SystemTimingChanged;
        }

        private void EmuCore_SystemTimingChanged(SystemTiming timing)
        {
            MusicPlayer.SetSampleRateAsync((uint)timing.SampleRate);
        }

        private void EmuCore_GameGeometryChanged(GameGeometry geometry)
        {
            FrameBuffer = new Texture2D(graphics.GraphicsDevice, (int)geometry.MaxWidth, (int)geometry.MaxHeight, false, SurfaceFormat.Bgr565);
        }

        private short EmuCore_GetInputState(uint port, InputTypes inputType)
        {
            return 0;
        }

        private void EmuCore_PollInput()
        {
        }

        private void EmuCore_RenderAudioFrames(short[] data, uint numFrames)
        {
            if(!MusicPlayer.Started)
            {
                MusicPlayer.Start();
            }

            MusicPlayer.AddSamples(data);
        }

        private void EmuCore_RenderVideoFrame(byte[] frameBuffer, uint width, uint height, uint pitch)
        {
            var targetArea = new Rectangle(0, 0, (int)EmuCore.Geometry.MaxWidth, (int)height);
            FrameBuffer.SetData<byte>(0, targetArea, frameBuffer, 0, frameBuffer.Length);        
        }

        public async void LoadRom(IStorageFile storageFile)
        {
            CoreShouldRun = false;
            MusicPlayer.Stop();
            await Task.Run(() =>
            {
                lock(EmuCore)
                {
                    EmuCore.LoadGame(storageFile);
                }
            });
            CoreShouldRun = true;
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
            if(CoreShouldRun)
            {
                lock (EmuCore)
                {
                    EmuCore.RunFrame();
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
                lock (EmuCore)
                {
                    var viewportLocation = ComputeBestFittingSize(new Point(viewport.Width, viewport.Height), EmuCore.Geometry.AspectRatio);
                    var frameBufferSize = new Point((int)EmuCore.Geometry.BaseWidth, (int)EmuCore.Geometry.BaseHeight);
                    spriteBatch.Draw(FrameBuffer, viewportLocation, new Rectangle(Point.Zero, frameBufferSize), Color.White);
                }
                //spriteBatch.DrawString(font, $"Frame {frameNumber}", new Vector2(0, 0), Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
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
    }
}
