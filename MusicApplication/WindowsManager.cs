using Engine;
using Engine.Commands;
using System;
using System.Windows;

namespace MusicApplication
{
    public static class WindowsManager
    {
        public static readonly darknet.Mode Mode = Helper.Utility.CustomThemeListener.IsDark ? darknet.Mode.Dark : darknet.Mode.Light;

        public static void ApplyWindowsTheme()
        {
            darknet.wpf.DarkNetWpfImpl darkNetWpfImpl = new();
            darkNetWpfImpl.SetModeForCurrentProcess(Mode);
            if (Mode == darknet.Mode.Dark)
            {
                ResourceDictionary Dark = new()
                {
                    Source = new Uri("..\\Resource\\Theme\\Dark.Xaml", UriKind.Relative)
                };
                Application.Current.Resources.MergedDictionaries[0] = Dark;
            }
        }

        public static void StartApp()
        {
            ApplyWindowsTheme();
            MainCommands.Initialize();
            PlaylistManager.Initialize();

            Windows.MainWindow mainWindow = new();
            mainWindow.Show();
            mainWindow.Closed += (_, _) => { Application.Current.Shutdown(); };
        }

        public static void WindowInitialized(Window Window)
        {
            _ = Window.Activate();
            Window.BringIntoView();

            MainCommands.Initialize();

            Window.MouseWheel += (_, e) =>
            {
                switch (e.Delta)
                {
                    case >= 0:
                        MainCommands.VolumeUp(0.05);
                        break;
                    default:
                        MainCommands.VolumeDown(0.05);
                        break;
                }
                e.Handled = true;
            };

            Window.AllowDrop = true;
            Window.DragEnter += (_, e) => e.Effects = DragDropEffects.All;
            Window.Drop += (_, e) =>
            {
                string[] dropitems = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                PlaylistManager.AddRangeAsync(0, dropitems);
                MainCommands.Source = dropitems[0];
            };
        }
    }
}
