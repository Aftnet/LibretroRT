using System;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Test
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        readonly Game1 _game;

        public GamePage()
        {
            this.InitializeComponent();

            // Create the game.
            var launchArguments = string.Empty;
            _game = MonoGame.Framework.XamlGame<Game1>.Create(launchArguments, Window.Current.CoreWindow, swapChainPanel);
        }

        private async void OpenRomButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            var extensions = _game.GetGenesisSupportedExtensions();
            foreach(var i in extensions)
            {
                picker.FileTypeFilter.Add(i);
            }
            var file = await picker.PickSingleFileAsync();
            if (file == null)
                return;

            _game.LoadRom(file);
        }

        private void SaveStateButton_Click(object sender, RoutedEventArgs e)
        {
            _game.SaveState();
        }

        private void LoadStateButton_Click(object sender, RoutedEventArgs e)
        {
            _game.LoadState();
        }
    }
}
