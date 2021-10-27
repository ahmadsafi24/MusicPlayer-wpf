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

        public static void SwitchToBlurWindow()
        {
            var lastwin = Application.Current.MainWindow;
            Windows.MainWindowChromeless Chromeless = new();
            Application.Current.MainWindow = Chromeless;
            lastwin.Close();
            Chromeless.Show();
        }
        
        public static void SwitchToNormalWindow()
        {
            var lastwin = Application.Current.MainWindow;
            Windows.MainWindow win = new();
            Application.Current.MainWindow = win;
            lastwin.Close();
            win.Show();
        }
    }
}
