using System.Windows.Controls;

namespace PlayerUI.Control
{
    public partial class PlayerControl : UserControl
    {
        private PlayerLibrary.SoundPlayer Player => App.Player;
        public PlayerControl()
        {
            InitializeComponent();
        }
    }
}
