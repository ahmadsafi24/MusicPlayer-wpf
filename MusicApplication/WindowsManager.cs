using Engine;
using Engine.Commands;
using Helper.DarkUi;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace MusicApplication
{
    internal static class WindowsManager
    {
        internal static bool isDark { get; set; } = Helper.CustomThemeListener.IsDark;

        [DllImport("uxtheme.dll", EntryPoint = "#135", SetLastError = true)]
        internal static extern bool SetPreferredAppMode(AppMode preferredAppMode);

        internal static void ApplyWindowsTheme()
        {
            if (isDark)
            {
                _ = SetPreferredAppMode(AppMode.ForceDark);
                ResourceDictionary Dark = new()
                {
                    Source = new Uri("..\\Resource\\Theme\\Dark.Xaml", UriKind.Relative)
                };
                Application.Current.Resources.MergedDictionaries[0] = Dark;
            }
            Debug.WriteLine("ApplyWindowsThemeCompleted");
        }
        internal static Windows.MainWindow mainWindow = new();
        internal static void StartApp(string[] args)
        {
            ApplyWindowsTheme();
            mainWindow.SourceInitialized += (_, _) =>
            {
                Helper.IconHelper.RemoveIcon(mainWindow);
                if (isDark)
                {
                    DwmApi.ToggleImmersiveDarkMode(mainWindow, true);
                }
            };
            mainWindow.Show();

            if (args?.Length > 0)
            {
                Task.Run(async () => await PlaylistManager.AddRangeAsync(0, args));
                MainCommands.Source = args[0];
                Task.Run(async () => await MainCommands.OpenAsync());
            }
        }

        internal static void WindowInitialized(Window Window)
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
                await MainCommands.OpenAsync();
            };
        }
    }
}
