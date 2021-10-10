using System.Windows.Controls;

namespace MusicApplication.Control
{
    /// <summary>
    /// Interaction logic for PlayerControl.xaml
    /// </summary>
    public partial class PlayerControl : UserControl
    {
        private readonly AudioPlayer.Player Player = App.Player;
        public PlayerControl()
        {
            InitializeComponent();
        }
    }
}
