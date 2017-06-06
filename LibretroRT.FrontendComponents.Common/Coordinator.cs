using System.Threading;

namespace LibretroRT.FrontendComponents.Common
{
    public sealed class Coordinator
    {
        private ICore core;
        public ICore Core
        {
            get { return core; }
            set
            {
                if (core != value)
                {
                    UnregisterCoreEvents();
                    core = value;
                    RegisterEvents();
                }
            }
        }

        private IRenderer renderer;
        public IRenderer Renderer
        {
            get { return renderer; }
            set
            {
                if (renderer != value)
                {
                    UnregisterCoreEvents();
                    renderer = value;
                    RegisterEvents();
                }
            }
        }

        private IAudioPlayer audioPlayer;
        public IAudioPlayer AudioPlayer
        {
            get { return audioPlayer; }
            set
            {
                if (audioPlayer != value)
                {
                    UnregisterCoreEvents();
                    audioPlayer = value;
                    RegisterEvents();
                }
            }
        }

        private IInputManager inputManager;
        public IInputManager InputManager
        {
            get { return inputManager; }
            set
            {
                if (inputManager != value)
                {
                    UnregisterCoreEvents();
                    inputManager = value;
                    RegisterEvents();
                }
            }
        }

        public bool AudioPlayerRequestsFrameDelay { get { return AudioPlayer != null && AudioPlayer.ShouldDelayNextFrame; } }

        private void UnregisterCoreEvents()
        {
            if (Core == null)
                return;

            if (Renderer != null)
            {
                Core.GeometryChanged -= Renderer.GeometryChanged;
                Core.PixelFormatChanged -= Renderer.PixelFormatChanged;
                Core.RenderVideoFrame -= Renderer.RenderVideoFrame;
            }

            if (AudioPlayer != null)
            {
                Core.TimingChanged -= AudioPlayer.TimingChanged;
                Core.RenderAudioFrames -= AudioPlayer.RenderAudioFrames;
            }

            if (InputManager != null)
            {
                Core.PollInput -= InputManager.PollInput;
                Core.GetInputState -= InputManager.GetInputState;
            }
        }

        private void RegisterEvents()
        {
            if (Core == null)
                return;

            if (Renderer != null)
            {
                Core.GeometryChanged += Renderer.GeometryChanged;
                Core.PixelFormatChanged += Renderer.PixelFormatChanged;
                Core.RenderVideoFrame += Renderer.RenderVideoFrame;
            }

            if (AudioPlayer != null)
            {
                Core.TimingChanged += AudioPlayer.TimingChanged;
                Core.RenderAudioFrames += AudioPlayer.RenderAudioFrames;
            }

            if (InputManager != null)
            {
                Core.PollInput += InputManager.PollInput;
                Core.GetInputState += InputManager.GetInputState;
            }
        }
    }
}
