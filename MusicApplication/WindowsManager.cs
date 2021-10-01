using System.Threading.Tasks;
using System.Windows;

namespace MusicApplication
{
    internal static class WindowsManager
    {
        internal static async void StartApp(string[] args)
        {
            await Task.Run(async () =>
            {
                if (args?.Length > 0)
                {
                    _ = Task.Run(async () => await SharedStatics.Player.Playlist.AddRangeAsync(args));
                    SharedStatics.Player.Source = args[0];
                    await SharedStatics.Player.OpenAsync();
                }
            });
        }

        internal static void WindowInitialized(Window Window)
        {
            _ = Window.Activate();
            _ = Window.Focus();
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
