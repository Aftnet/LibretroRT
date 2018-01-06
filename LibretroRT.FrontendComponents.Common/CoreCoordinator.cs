using System;

namespace LibretroRT.FrontendComponents.Common
{
    public sealed class CoreCoordinator : IDisposable
    {
        private ICore core;
        public ICore Core
        {
            get { return core; }
            set
            {
                if (core != value)
                {
                    UnregisterEvents();
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
                    UnregisterEvents();
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
                    UnregisterEvents();
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
                    UnregisterEvents();
                    inputManager = value;
                    RegisterEvents();
                }
            }
        }

        public bool AudioPlayerRequestsFrameDelay { get { return AudioPlayer != null && AudioPlayer.ShouldDelayNextFrame; } }

        public void Dispose()
        {
            Core = null;
            AudioPlayer = null;
            InputManager = null;
        }

        private void UnregisterEvents()
        {
            if (Core == null)
                return;

            if (Renderer != null)
            {
                Core.GeometryChanged -= Renderer.GeometryChanged;
                Core.PixelFormatChanged -= Renderer.PixelFormatChanged;
                Core.RenderVideoFrame -= Renderer.RenderVideoFrame;
                Core.TimingChanged -= Renderer.TimingChanged;
            }

            if (AudioPlayer != null)
            {
                Core.TimingChanged -= AudioPlayer.TimingChanged;
                Core.RenderAudioFrames -= AudioPlayer.RenderAudioFrames;
            }

            Core.PollInput = null;
            Core.GetInputState = null;
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
                Core.TimingChanged += Renderer.TimingChanged;
            }

            if (AudioPlayer != null)
            {
                Core.TimingChanged += AudioPlayer.TimingChanged;
                Core.RenderAudioFrames += AudioPlayer.RenderAudioFrames;
            }

            if (InputManager != null)
            {
                Core.PollInput = InputManager.PollInput;
                Core.GetInputState = InputManager.GetInputState;
            }
        }
    }
}
