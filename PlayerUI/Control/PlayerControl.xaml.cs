using System.Windows.Controls;

namespace PlayerUI.Control
{
    public partial class PlayerControl : UserControl
    {
        private PlayerLibrary.Player Player => App.Player;
        public PlayerControl()
        {
            InitializeComponent();
        }
    }
}
