using System.Windows;

namespace PlayerUI.Commands
{
    public static class ViewSwitcher
    {
        public static void SwitchToMiniView()
        {
            System.Windows.Window lastwin = Application.Current.MainWindow;
            Windows.ControlbarWindows ControlBarWindows = new();
            ControlBarWindows.Show();
            Application.Current.MainWindow = ControlBarWindows;
            lastwin.Close();
        }

        public static void SwitchToMainView()
        {
            var lastwin = Application.Current.MainWindow;
            Windows.MainWindow MainWindow = new();
            MainWindow.Show();
            Application.Current.MainWindow = MainWindow;
            lastwin.Close();
        }
    }
}
