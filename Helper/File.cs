using System.Drawing;
using System.Windows.Media.Imaging;

namespace Helper
{
    public static class File
    {
        public static void SaveBitmapImageToPng(BitmapImage image, string destinationFilePath)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using System.IO.FileStream filestream = new(destinationFilePath, System.IO.FileMode.Create);
            encoder.Save(filestream);
        }

        /*public static void SaveImageToPng(Image image, string destinationFilePath)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using System.IO.FileStream filestream = new(destinationFilePath, System.IO.FileMode.Create);
            encoder.Save(filestream);
        }*/

        public static void OpenFileWithDefaultApp(string filepath)
        {
            if (System.IO.File.Exists(filepath))
            {
                System.Diagnostics.Process start = new()
                {
                    StartInfo = new(filepath)
                    {
                        CreateNoWindow = true,
                        UseShellExecute = true,
                    }
                };
                start.Start();
            }
        }
    }
}
