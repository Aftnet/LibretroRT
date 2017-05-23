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
        private readonly ICore EmuCore = GPGXRT.GPGXCore.Instance;

        private Texture2D FrameBuffer = null;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            EmuCore.RenderVideoFrame += EmuCore_RenderVideoFrame;
            EmuCore.RenderAudioFrames += EmuCore_RenderAudioFrames;
            EmuCore.PollInput += EmuCore_PollInput;
            EmuCore.GetInputState += EmuCore_GetInputState;
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
        }

        private void EmuCore_RenderVideoFrame(byte[] frameBuffer, uint width, uint height, uint pitch)
        {
            var texture = new Texture2D(graphics.GraphicsDevice, (int)width, (int)height, false, SurfaceFormat.ColorSRgb);
            texture.SetData<byte>(frameBuffer);          
        }

        public void LoadRom(IStorageFile storageFile)
        {
            Task.Run(() =>
            {
                lock(EmuCore)
                {
                    EmuCore.LoadGame(storageFile);
                }
            });
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
            lock(EmuCore)
            {
                EmuCore.RunFrame();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            if (FrameBuffer != null)
            {
                spriteBatch.Begin();
                lock(EmuCore)
                {
                    spriteBatch.Draw(FrameBuffer, new Rectangle(0, 0, 800, 600), Color.White);
                }
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
