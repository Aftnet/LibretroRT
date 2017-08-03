using RetriX.Shared.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace RetriX.UWP.Controls
{
    public sealed partial class PlayerOverlay : UserControl
    {
        public GamePlayerVM ViewModel
        {
            get { return (GamePlayerVM)GetValue(VMProperty); }
            set { SetValue(VMProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VM.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VMProperty = DependencyProperty.Register(nameof(ViewModel), typeof(GamePlayerVM), typeof(PlayerOverlay), new PropertyMetadata(null));

        public PlayerOverlay()
        {
            InitializeComponent();
        }
    }
}
