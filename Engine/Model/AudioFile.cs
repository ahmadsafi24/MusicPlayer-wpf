using System.IO;
using System.Windows.Media.Imaging;
using ATL;

namespace Engine.Model
{
    public class AudioFile
    {
        public string FilePath { get; set; }
        public AudioFile()
        {
        }

        public AudioFile(string file)
        {
            if (File.Exists(file))
            {
                FilePath = file;
            }
        }

        public bool FileExist => File.Exists(FilePath);
        public string FileName => Path.GetFileName(FilePath);
        public string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(FilePath);
        public string FileExtension => Path.GetExtension(FilePath);
        public string FileDirectory => Path.GetDirectoryName(FilePath);
        public string Title => new Track(FilePath).Title;
        public string Artist => new Track(FilePath).Artist;
        public string Album => new Track(FilePath).Album;
        public string AlbumArtist => new Track(FilePath).AlbumArtist;
        public BitmapImage Cover                          //=> Image.FromStream(new MemoryStream(tagfile.EmbeddedPictures[0]?.PictureData));
        {
            get
            {
                if (FilePath != null)
                {
                    Track tagfile = new(FilePath);
                    if (tagfile.EmbeddedPictures.Count > 0)
                    {
                        MemoryStream ms = new(tagfile.EmbeddedPictures[0]?.PictureData);
                        _ = ms.Seek(0, SeekOrigin.Begin);
                        BitmapImage bitmap = new();

                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.CreateOptions = BitmapCreateOptions.DelayCreation;

                        bitmap.BeginInit();
                        bitmap.StreamSource = ms;
                        bitmap.EndInit();
                        return bitmap;
                    }
                }
                return null;
            }
        }
        public static AudioFile Empty = new();
    }

    /* public TagFile TagFileEmpty
     {
         public bool FileExist { get; }
         public string FilePath { get; } = "Nofile";
         public string FileName => FilePath;
         public string FileNameWithoutExtension => FilePath;
         public string FileExtension => FilePath;
         public string FileDirectory => FilePath;
         public string Title => FilePath;
         public string Artist => FilePath;
         public string Album => FilePath;
         public string AlbumArtist => FilePath;
         public static BitmapImage Cover => null;
     }
    */
}