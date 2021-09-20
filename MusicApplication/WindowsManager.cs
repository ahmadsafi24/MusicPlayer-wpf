using Engine;
using Helper.DarkUi;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MusicApplication
{
    internal static class WindowsManager
    {
        internal static Windows.MainWindow mainWindow = new();

        [DllImport("uxtheme.dll", EntryPoint = "#135", SetLastError = true)]
        internal static extern bool SetPreferredAppMode(AppMode preferredAppMode);

        internal static void StartApp(string[] args)
        {
            //ApplyWindowsTheme();
            mainWindow.SourceInitialized += (_, _) =>
            {
                Helper.IconHelper.RemoveIcon(mainWindow);

            };
            mainWindow.Show();
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                if (args?.Length > 0)
                {
                    _ = Task.Run(async () => await PlaylistManager.AddRangeAsync(0, args));
                    Player.Source = args[0];
                    _ = Task.Run(async () => await Player.OpenAsync());
                }
            });
        }

        internal static void WindowInitialized(Window Window)
        {
            _ = Window.Activate();
            Window.BringIntoView();


            Window.MouseWheel += (_, e) =>
            {
                switch (e.Delta)
                {
                    case >= 0:
                        Player.VolumeUp(5);
                        break;
                    default:
                        Player.VolumeDown(5);
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
                Player.Source = dropitems[0];
                await Player.OpenAsync();
            };
        }
    }
}
