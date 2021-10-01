using MusicApplication.View;
using System.Windows;

namespace MusicApplication.Commands
{
    public static class ViewSwitcher
    {
        public static void SwitchToMiniView()
        {
            Windows.ControlbarWindows ControlBarWindows = new();
            ControlBarWindows.Show();
            ControlBarWindows.Content = AllViews.MiniView;
            var lastwin = Application.Current.MainWindow;
            Application.Current.MainWindow = ControlBarWindows;
            lastwin.Close();
        }

        public static void SwitchToMainView()
        {
            Windows.MainWindow MainWindow = new();
            MainWindow.Show();
            MainWindow.Content = AllViews.MainView;
            var lastwin = Application.Current.MainWindow;
            Application.Current.MainWindow = MainWindow;
            lastwin.Close();
        }
    }
}
