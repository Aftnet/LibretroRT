using LibretroRT.FrontendComponents.Common;
using MonoGame.Framework;
using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LibretroRT.FrontendComponents.MonoGameRenderer
{
    public sealed class MonoGameCoreRunner : ICoreRunner, IDisposable
    {
        private readonly MonoGameRenderer Renderer;

        public MonoGameCoreRunner(SwapChainPanel swapChainPanel, IAudioPlayer audioPlayer, IInputManager inputManager)
        {
            Renderer = XamlGame<MonoGameRenderer>.Create(string.Empty, Window.Current.CoreWindow, swapChainPanel);
        }

        public void Dispose()
        {
            Renderer.Dispose();
        }

        public void LoadGame(ICore core, IStorageFile gameFile)
        {
            Renderer.LoadGame(core, gameFile);
        }

        public void PauseCoreExecution()
        {
            Renderer.PauseCoreExecution();
        }

        public void ResetGame()
        {
            Renderer.ResetGame();
        }

        public void ResumeCoreExecution()
        {
            Renderer.ResumeCoreExecution();
        }

        public void UnloadGame()
        {
            Renderer.UnloadGame();
        }
    }
}
