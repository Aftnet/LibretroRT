using LibretroRT.FrontendComponents.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;

namespace LibretroRT.FrontendComponents.MonoGameRenderer
{
    internal class MonoGameRenderer : Game, IRenderer, ICoreRunner, IDisposable
    {
        private readonly GraphicsDeviceManager DeviceManager;
        private GraphicsDevice Device => DeviceManager.GraphicsDevice;

        private readonly CoreEventCoordinator Coordinator;

        public IAudioPlayer AudioPlayer
        {
            get { return Coordinator.AudioPlayer; }
            set { Coordinator.AudioPlayer = value; }
        }

        public IInputManager InputManager
        {
            get { return Coordinator.InputManager; }
            set { Coordinator.InputManager = value; }
        }

        private bool RunCore { get; set; }

        private SpriteBatch SpriteBatch { get; set; }
        private RenderTargetManager RenderTargetManager { get; set; }

        public MonoGameRenderer()
        {
            Coordinator = new CoreEventCoordinator { Renderer = this };

            RunCore = false;
            DeviceManager = new GraphicsDeviceManager(this);
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
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            RenderTargetManager = new RenderTargetManager();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            RenderTargetManager.Dispose();
            SpriteBatch.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            lock (Coordinator)
            {
                if (RunCore && !Coordinator.AudioPlayerRequestsFrameDelay)
                {
                    Coordinator.Core?.RunFrame();
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin();
            RenderTargetManager.Render(SpriteBatch, Device.Viewport.Bounds.Size);
            SpriteBatch.End();

            base.Draw(gameTime);
        }

        public void LoadGame(ICore core, IStorageFile gameFile)
        {
            lock (Coordinator)
            {
                Coordinator.Core?.UnloadGame();
                Coordinator.Core = core;
                Coordinator.Core.LoadGame(gameFile);
                RunCore = true;
            }
        }

        public void UnloadGame()
        {
            lock (Coordinator)
            {
                RunCore = false;
                Coordinator.Core?.UnloadGame();
            }
        }

        public void ResetGame()
        {
            lock (Coordinator)
            {
                Coordinator.Core?.Reset();
            }
        }

        public void PauseCoreExecution()
        {
            lock (Coordinator)
            {
                RunCore = false;
            }
        }

        public void ResumeCoreExecution()
        {
            lock (Coordinator)
            {
                RunCore = true;
            }
        }

        public void GeometryChanged(GameGeometry geometry)
        {
            var core = Coordinator.Core;
            RenderTargetManager.UpdateFormat(Device, core.Geometry, core.PixelFormat);
        }

        public void PixelFormatChanged(PixelFormats format)
        {
            var core = Coordinator.Core;
            RenderTargetManager.UpdateFormat(Device, core.Geometry, core.PixelFormat);
        }

        public void RenderVideoFrame([ReadOnlyArray] byte[] frameBuffer, uint width, uint height, uint pitch)
        {
            RenderTargetManager.UpdateFromCoreOutput(frameBuffer, width, height, pitch);
        }
    }
}
