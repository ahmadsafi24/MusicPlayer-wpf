using Helper.DarkUi;
using System;
using System.Windows;

namespace MusicApplication
{
    public static class WindowTheme
    {
        private static bool IsDark = false;
        public static void DarkThemeToggle()
        {
            if (IsDark)
            {
                _ = WindowsManager.SetPreferredAppMode(AppMode.ForceLight);
                ResourceDictionary Light = new()
                {
                    Source = new Uri("..\\Resource\\Theme\\Light.Xaml", UriKind.Relative)
                };
                Application.Current.Resources.MergedDictionaries[0] = Light;
                foreach (Window item in WindowsManager.WindowList)
                {
                    if (item.IsVisible)
                    {
                        DwmApi.ToggleImmersiveDarkMode(item, false);
                    }
                }
            }
            else
            {
                _ = WindowsManager.SetPreferredAppMode(AppMode.ForceDark);
                ResourceDictionary Dark = new()
                {
                    Source = new Uri("..\\Resource\\Theme\\Dark.Xaml", UriKind.Relative)
                };
                Application.Current.Resources.MergedDictionaries[0] = Dark;
                foreach (Window item in WindowsManager.WindowList)
                {
                    if (item.IsVisible)
                    {
                        DwmApi.ToggleImmersiveDarkMode(item, true);
                    }
                }
            }
            IsDark = !IsDark;

            //WindowsManager.mainWindow.Hide();
            //WindowsManager.mainWindow.Show();



            /* Playlist playlist = new Playlist();
             System.Windows.Window window = new();
             window.BeginInit();
             playlist.DataContext = new PlaylistViewModel();
             window.Content = playlist;
             playlist.InitializeComponent();
             window.EndInit();
             window.Show();*/
        }
    }
}
