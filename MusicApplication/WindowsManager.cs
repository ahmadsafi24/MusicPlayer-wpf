using Engine;
using Engine.Commands;
using Helper.DarkUi;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace MusicApplication
{
    public static class WindowsManager
    {
        public static readonly bool isDark = Helper.CustomThemeListener.IsDark;

        [DllImport("uxtheme.dll", EntryPoint = "#135", SetLastError = true)]
        internal static extern bool SetPreferredAppMode(AppMode preferredAppMode);

        public static void ApplyWindowsTheme()
        {
            _ = SetPreferredAppMode(AppMode.ForceDark);
            if (isDark)
            {
                ResourceDictionary Dark = new()
                {
                    Source = new Uri("..\\Resource\\Theme\\Dark.Xaml", UriKind.Relative)
                };
                Application.Current.Resources.MergedDictionaries[0] = Dark;
            }
            Debug.WriteLine("ApplyWindowsThemeCompleted");
        }

        public static void StartApp(string[] args)
        {
            ApplyWindowsTheme();
            MainCommands.Initialize();
            PlaylistManager.Initialize();

            Windows.MainWindow mainWindow = new();
            mainWindow.SourceInitialized += (_, _) =>
            {
                Helper.IconHelper.RemoveIcon(mainWindow);
                if (isDark)
                {
                    DwmApi.ToggleImmersiveDarkMode(mainWindow, true);
                }
            };
            mainWindow.Show();
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
            Window.Drop += async (_, e) =>
            {
                string[] dropitems = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                await PlaylistManager.AddRangeAsync(0, dropitems);
                MainCommands.Source = dropitems[0];
            };
        }
    }
}
