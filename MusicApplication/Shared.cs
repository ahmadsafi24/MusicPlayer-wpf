using Engine;
using System.Diagnostics;

namespace MusicApplication
{
    public static class Shared
    {
        internal static Player Player = new();
        internal static void OpenCurrentFileLocation()
        {
            Helper.OpenFileLocation.Open(Player.Source);
        }

        internal const string stringformat = "mm\\:ss";
        internal static async void OpenFilePicker()
        {
            Debug.WriteLine("opening OpenFilePicker");
            string[] files = Helper.FileOpenPicker.GetFiles();
            if (files.Length > 0)
            {
               // await Shared.Player.PlaylistManager.AddRangeAsync(0, files);
                Player.Source = files[0];
                await Player.OpenAsync();
            }

        }
    }

}
