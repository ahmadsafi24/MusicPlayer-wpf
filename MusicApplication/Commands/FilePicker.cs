using System.Diagnostics;

namespace MusicApplication.Commands
{
    public static class FilePicker
    {
        private static readonly AudioPlayer.Player Player = MusicApplication.App.Player;

        internal static async void OpenFilePicker()
        {
            Debug.WriteLine("Opening OpenFilePicker");
            string[] files = await Helper.FileOpenPicker.GetFileAsync();
            if (files.Length > 0)
            {
                await Player.Playlist.AddRangeAsync(files);
                Player.Source = files[0];
                await Player.OpenAsync();
            }
        }
    }
}
