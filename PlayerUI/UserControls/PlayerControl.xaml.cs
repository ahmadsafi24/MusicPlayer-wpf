using System.Windows.Controls;

namespace PlayerUI.UserControls
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
