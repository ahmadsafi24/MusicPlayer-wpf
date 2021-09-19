using Engine;
using System.Diagnostics;

namespace MusicApplication
{
    public static class Shared
    {
        internal static void OpenCurrentFileLocation()
        {
            Helper.OpenFileLocation.Open(Player.Source);
        }

        internal const string stringformat = "mm\\:ss";
        internal static async void OpenFilePicker()
        {
            Debug.WriteLine("opening OpenFilePicker");
            string[] files = await Helper.FileOpenPicker.GetFileAsync();
            if (files.Length > 0)
            {
                await PlaylistManager.AddRangeAsync(0, files);
                Player.Source = files[0];
                await Player.OpenAsync();
            }

        }
    }

}
