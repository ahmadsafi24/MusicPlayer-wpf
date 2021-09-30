using AudioPlayer;
using System.Diagnostics;

namespace MusicApplication
{
    public static class SharedStatics
    {
        internal const string stringformat = "mm\\:ss";

        internal static Player Player = new();

        internal static void OpenCurrentFileLocation()
        {
            Helper.OpenFileLocation.Open(Player.Source);
        }

        internal static async void OpenFilePicker()
        {
            Debug.WriteLine("opening OpenFilePicker");
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
