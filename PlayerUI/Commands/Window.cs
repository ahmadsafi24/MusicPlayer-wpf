using PlayerLibrary.Preset;
using System.Windows;

namespace PlayerUI.Commands
{
    public static class Window
    {
        private static PlayerLibrary.Player Player => PlayerUI.App.Player;
        public static void AttachDrop(System.Windows.Window window)
        {
            window.AllowDrop = true;
            window.DragEnter += (_, e) => e.Effects = DragDropEffects.All;
            window.Drop += async (_, e) =>
            {
                string[] dropitems = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                if (System.IO.Path.GetExtension(dropitems[0]) == ".EqPreset")
                {
                    Player.ImportEq(Equalizer.PresetFromFile(dropitems[0]));
                }
                else
                {
                    await Player.Controller.OpenAsync(dropitems[0]);
                }
            };
        }

        public static void AttachMouseWheel(System.Windows.Window window)
        {
            window.MouseWheel += (_, e) => MouseWheelChanged(e);
        }

        private static void MouseWheelChanged(System.Windows.Input.MouseWheelEventArgs e)
        {
            switch (e.Delta)
            {
                case > 0:
                    Player.VolumeUp(5);
                    break;
                default:
                    Player.VolumeDown(5);
                    break;
            }
        }
    }
}
