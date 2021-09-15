using Engine;
using Engine.Commands;
using System.Configuration;

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
            string[] files = await Helper.FileOpenPicker.GetFileAsync();
            await PlaylistManager.AddRangeAsync(0, files);

            MainCommands.Source = files[0];
            await MainCommands.OpenAsync();
        }
    }

}
