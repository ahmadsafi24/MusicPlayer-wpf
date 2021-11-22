using ATL;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PlayerLibrary.Utility
{
    public static class AlbumArt
    {
        public static BitmapImage ExtractCover(string filePath)
        {
            BitmapImage bitmap = null;

            if (filePath != null)
            {
                Track tagfile = new(filePath);
                if (tagfile.EmbeddedPictures.Count > 0)
                {
                    if (tagfile.PictureTokens?.Count > 0 &&
                        tagfile.EmbeddedPictures?.Count > 0)
                    {
                        if (tagfile.EmbeddedPictures[0]?.PictureData.Length > 3)
                        {
                            Helper.Log.WriteLine("cover reading");

                            MemoryStream ms = new(tagfile.EmbeddedPictures[0]?.PictureData);
                            _ = ms.Seek(0, SeekOrigin.Begin);

                            bitmap = new();
                            bitmap.BeginInit();

                            bitmap.StreamSource = ms;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;

                            bitmap.EndInit();
                            bitmap.Freeze();
                            ms.Flush();
                            ms.Dispose();
                        }
                    }
                }
            }

            return bitmap;
        }

        public static async Task<BitmapImage> AlbumArtAsync(string filePath)
        {
            return await Task.Run(() => ExtractCover(filePath));
        }
    }
}
