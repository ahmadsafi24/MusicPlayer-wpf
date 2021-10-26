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
            Chromeless.Width=lastwin.Width;
            Chromeless.Height=lastwin.Height;
            Chromeless.Left=lastwin.Left;
            Chromeless.Top=lastwin.Top;
            Chromeless.Show();
            Application.Current.MainWindow = Chromeless;
            lastwin.Close();
        }
        
        public static void SwitchToNormalWindow()
        {
            var lastwin = Application.Current.MainWindow;
            Windows.MainWindow win = new();
            win.Width=lastwin.Width;
            win.Height=lastwin.Height;
            win.Left=lastwin.Left;
            win.Top=lastwin.Top;
            win.Show();
            Application.Current.MainWindow = win;
            lastwin.Close();
        }
    }
}
