using Helper.DarkUi;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MusicApplication
{
    internal static class WindowsManager
    {
        [DllImport("uxtheme.dll", EntryPoint = "#135", SetLastError = true)]
        internal static extern bool SetPreferredAppMode(AppMode preferredAppMode);

        internal static Windows.MainWindow mainWindow = new();
        internal static Windows.Window1 playercontrolwin = new();

        internal static List<Window> WindowList = new();

        internal static void StartApp(string[] args)
        {

            WindowList.Add(mainWindow);
            WindowList.Add(playercontrolwin);

            mainWindow.SourceInitialized += MainWindow_SourceInitialized;

            mainWindow.Show();
            playercontrolwin.Show();

            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                mainWindow.Content = new View.MainView();
                if (args?.Length > 0)
                {
                    //_ = Task.Run(async () => await PlaylistManager.AddRangeAsync(0, args));
                    SharedStatics.Player.Source = args[0];
                    _ = Task.Run(async () => await SharedStatics.Player.OpenAsync());
                }
            });

            WindowTheme.DarkThemeToggle();
        }

        private static void MainWindow_SourceInitialized(object sender, System.EventArgs e)
        {
            Helper.IconHelper.RemoveIcon(mainWindow);
        }

        internal static void WindowInitialized(Window Window)
        {
            _ = Window.Activate();
            Window.BringIntoView();


            Window.MouseWheel += (_, e) => Window_MouseWheel(e);

            Window.AllowDrop = true;
            Window.DragEnter += (_, e) => e.Effects = DragDropEffects.All;
            Window.Drop += (_, e) => Window_Drop(e);
            {
            };
        }

        private static async void Window_Drop(DragEventArgs e)
        {

            string[] dropitems = (string[])e.Data.GetData(DataFormats.FileDrop, true);
            //await SharedStatics.Player.PlaylistManager.AddRangeAsync(0, dropitems);
            SharedStatics.Player.Source = dropitems[0];
            await SharedStatics.Player.OpenAsync();
        }

        private static void Window_MouseWheel(System.Windows.Input.MouseWheelEventArgs e)
        {
            switch (e.Delta)
            {
                case > 0:
                    SharedStatics.Player.VolumeUp(5);
                    break;
                default:
                    SharedStatics.Player.VolumeDown(5);
                    break;
            }
            e.Handled = true;
        }
    }
}
