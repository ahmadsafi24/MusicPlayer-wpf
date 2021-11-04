﻿using ATL;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PlayerLibrary.Utility
{
    public static class CoverImage
    {
        public static BitmapImage ExtractCoverFastRender(string filePath)
        {
            BitmapImage bitmap = null;

            if (filePath != null)
            {
                Track tagfile = new(filePath);
                if (tagfile.EmbeddedPictures.Count > 0)
                {
                    byte[] pic = tagfile.EmbeddedPictures[0]?.PictureData;
                    System.IO.MemoryStream ms = new(pic);
                    _ = ms.Seek(0, System.IO.SeekOrigin.Begin);

                    bitmap = new();
                    bitmap.BeginInit();

                    bitmap.StreamSource = ms;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.DecodePixelWidth = 50;
                    bitmap.DecodePixelHeight = 50;

                    bitmap.EndInit();
                    bitmap.Freeze();
                    _ = ms.DisposeAsync();
                }
            }

            return bitmap;
        }
        public static BitmapImage ExtractCover(string filePath)
        {
            BitmapImage bitmap = null;

            if (filePath != null)
            {
                Track tagfile = new(filePath);
                if (tagfile.PictureTokens.Count >= 1)
                {
                    if (tagfile.EmbeddedPictures.Count >= 1)
                    {
                        byte[] pic = tagfile.EmbeddedPictures[0]?.PictureData;
                        System.IO.MemoryStream ms = new(pic);
                        _ = ms.Seek(0, System.IO.SeekOrigin.Begin);

                        bitmap = new();
                        bitmap.BeginInit();

                        bitmap.StreamSource = ms;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;

                        bitmap.EndInit();
                        bitmap.Freeze();
                        ms.Flush();
                    }
                }
            }

            return bitmap;
        }

        public static async Task<BitmapImage> AlbumArtAsync(string filePath)
        {
            return await Task.Run<BitmapImage>(() => ExtractCover(filePath));
        }
    }

    public class CoverImage2
    {
        public event EventHandlerImage OnImageCreated;
        public async void CreateImage(string File)
        {
            await Task.Run(() => OnImageCreated?.Invoke(CoverImage.ExtractCover(File)));
        }
    }

    public delegate void EventHandlerImage(BitmapImage image);
}
