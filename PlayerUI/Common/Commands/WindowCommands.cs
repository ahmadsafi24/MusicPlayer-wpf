namespace PlayerUI.Common.Commands
{
    public static class WindowCommands
    {
        private static PlayerLibrary.Player Player => PlayerUI.App.Player;
        private static VolumeController VolumeController => Player.PlaybackSession.VolumeController;

        public static void AttachDrop(System.Windows.Window window)
        {
            window.AllowDrop = true;
            window.DragEnter += (_, e) => e.Effects = DragDropEffects.All;
            window.Drop += async (_, e) =>
            {
                var data = e.Data.GetData(DataFormats.FileDrop, true);
                if (data == null)
                {
                    data = e.Data.GetData(DataFormats.Html, true); //html for chrome
                    MessageBox.Show(data.ToString());
                    return;
                }
                Helper.Log.WriteLine(data.ToString());
                string[] dropitems = (string[])data;
                if (System.IO.Path.GetExtension(dropitems[0]) == ".EqPreset")
                {
                    Player.PlaybackSession.EffectContainer.EqualizerController.SetEqPreset(Equalizer.FileToPreset(dropitems[0]));
                }
                else
                {
                    await Player.PlaybackSession.OpenAsync(new Uri(dropitems[0]));
                }
            };
        }

        public static void AttachMouseWheel(System.Windows.Window window)
        {
            window.MouseWheel += MouseWheelChanged;
        }

        private static void MouseWheelChanged(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                VolumeController.VolumeUp((float)0.05);
            }
            else
            {
                VolumeController.VolumeDown((float)0.05);
            }
        }
    }
}
