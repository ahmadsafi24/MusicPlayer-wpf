namespace PlayerUI.Common.Commands
{
    public static class ViewSwitcher
    {
        public static void SwitchToMiniView()
        {
            Window lastwin = Application.Current.MainWindow;
            ControlbarWindows ControlBarWindows = new();
            ControlBarWindows.Show();
            Application.Current.MainWindow = ControlBarWindows;
            lastwin.Close();
        }

        public static void SwitchToMainView()
        {
            var lastwin = Application.Current.MainWindow;
            MainWindow MainWindow = new();
            MainWindow.Show();
            Application.Current.MainWindow = MainWindow;
            lastwin.Close();
        }

        public static void SwitchToBlurWindow()
        {
            var lastwin = Application.Current.MainWindow;
            CustomWindow CustomWindow = new();
            Application.Current.MainWindow = CustomWindow;
            lastwin.Close();
            CustomWindow.Show();
        }

        public static void SwitchToNormalWindow()
        {
            var lastwin = Application.Current.MainWindow;
            MainWindow win = new();
            Application.Current.MainWindow = win;
            lastwin.Close();
            win.Show();
        }
    }
}
