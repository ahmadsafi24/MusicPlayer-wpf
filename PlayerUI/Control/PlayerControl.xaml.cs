using System.Windows.Controls;

namespace PlayerUI.Control
{
    /// <summary>
    /// Interaction logic for PlayerControl.xaml
    /// </summary>
    public partial class PlayerControl : UserControl
    {
        private PlayerLibrary.Player Player => App.Player;
        public PlayerControl()
        {
            InitializeComponent();
        }
    }
}
