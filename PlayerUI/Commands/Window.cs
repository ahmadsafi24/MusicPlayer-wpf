using PlayerLibrary.Preset;
using PlayerLibrary.Core;
using System.Windows;
using System.Windows.Input;
using System;

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
            window.MouseWheel += MouseWheelChanged;
        }

        private static void MouseWheelChanged(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                volumeController.VolumeUp((float)0.05);
            }
            else
            {
                volumeController.VolumeDown((float)0.05);
            }
        }
    }
}
