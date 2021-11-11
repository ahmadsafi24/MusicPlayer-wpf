using PlayerLibrary.Preset;
using PlayerLibrary.Core;
using System.Windows;

namespace PlayerUI.Commands
{
    public static class Window
    {
        private static PlayerLibrary.Player Player => PlayerUI.App.Player;
        private static VolumeController volumeController => Player.PlaybackSession.VolumeController;

        public static void AttachDrop(System.Windows.Window window)
        {
            window.AllowDrop = true;
            window.DragEnter += (_, e) => e.Effects = DragDropEffects.All;
            window.Drop += async (_, e) =>
            {
                string[] dropitems = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                if (System.IO.Path.GetExtension(dropitems[0]) == ".EqPreset")
                {
                    Player.EqualizerController.SetEqPreset(Equalizer.FileToPreset(dropitems[0]));
                }
                else
                {
                    await Player.PlaybackSession.OpenAsync(dropitems[0]);
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
                    volumeController.VolumeUp(5);
                    break;
                default:
                    volumeController.VolumeDown(5);
                    break;
            }
        }
    }
}
