using System.Diagnostics;

namespace PlayerUI.Commands
{
    public static class FilePicker
    {
        private static readonly PlayerLibrary.Player Player = PlayerUI.App.Player;

        internal static async void OpenFilePicker()
        {
            Debug.WriteLine("Opening OpenFilePicker");
            string[] files = await Helper.FileOpenPicker.GetFileAsync();
            if (files.Length > 0)
            {
                string Source = files[0];
                await Player.OpenAsync(Source);
            }
        }
    }
}
