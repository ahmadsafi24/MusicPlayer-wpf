using System;
using System.Windows;

namespace MusicApplication
{
    public static class WindowsManager
    {
        public static darknet.Mode Mode => Helper.Utility.CustomThemeListener.Getmode() ? darknet.Mode.Dark : darknet.Mode.Light;

        public static void StartApp()
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
            Windows.MainWindow mainWindow = new();
            mainWindow.Show();
        }

        public static void WindowInitialized(Window Window)
        {
            _ = Window.Activate();
            Window.BringIntoView();

            Engine.Commands.MainCommands.Initialize();

            Window.MouseWheel += (_, e) =>
            {
                switch (e.Delta)
                {
                    case >= 0:
                        Engine.Commands.MainCommands.VolumeUp(0.05);
                        break;
                    default:
                        Engine.Commands.MainCommands.VolumeDown(0.05);
                        break;
                }
                e.Handled = true;
            };

            Window.AllowDrop = true;
            Window.DragEnter += (_, e) => e.Effects = DragDropEffects.All;
            Window.Drop += (_, e) =>
            {
                string[] dropitems = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                //Player.Playlist.CreateNewAndAdd(dropitems);
                Engine.Commands.MainCommands.Source = dropitems[0];
            };
        }
    }
}
