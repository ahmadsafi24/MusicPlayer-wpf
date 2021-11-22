namespace PlayerUI.Common.Commands
{
    public static class FilePicker
    {
        internal static async void OpenFilePicker(PlayerLibrary.Player player)
        {
            string[] files = await Helper.FileOpenPicker.GetFileAsync();
            if (files.Length > 0)
            {
                string Source = files[0];
                await player.PlaybackSession.OpenAsync(Source);
            }
        }
    }
}
