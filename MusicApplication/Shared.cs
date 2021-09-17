using Engine;
using Engine.Commands;
using System.Configuration;
using System.Diagnostics;

namespace MusicApplication
{
    public static class Shared
    {
        internal static void OpenCurrentFileLocation()
        {
            Helper.OpenFileLocation.Open(MainCommands.Source);
        }

        internal static async void OpenFilePicker()
        {
            Debug.WriteLine("opening OpenFilePicker");
            string[] files = await Helper.FileOpenPicker.GetFileAsync();
            if (files.Length > 0)
            {
                await PlaylistManager.AddRangeAsync(0, files);
                MainCommands.Source = files[0];
                await MainCommands.OpenAsync();
            }

        }
    }

}
