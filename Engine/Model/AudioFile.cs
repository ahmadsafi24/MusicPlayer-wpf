using System.Windows.Media.Imaging;
using ATL;

namespace Engine.Model
{
    public class AudioFile
    {
        public string FilePath;

        public AudioFile(string file)
        {
            if (System.IO.File.Exists(file))
            {
                FilePath = file;
            }

            Track t = new(file);
            Title = t.Title;
            Artist = t.Artist;
            Album = t.Album;
            AlbumArtist = t.AlbumArtist;

        }

        public string FileName => System.IO.Path.GetFileName(FilePath);
        //public string FileNameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(FilePath);
        //public string FileExtension => System.IO.Path.GetExtension(FilePath);
        //public string FileDirectory => System.IO.Path.GetDirectoryName(FilePath);
        public string Title { get; }
        public string Artist { get; }
        public string Album { get; }
        public string AlbumArtist { get; }
        private System.Threading.Tasks.Task<BitmapImage> _cover;
        public System.Threading.Tasks.Task<BitmapImage> Cover                          //=> Image.FromStream(new MemoryStream(tagfile.EmbeddedPictures[0]?.PictureData));
        {
            get
            {
                if (_cover == null)
                {
                    var temp = System.Threading.Tasks.Task.Run(() => Utility.Class1.ExtractCoverFastRender(FilePath));
                    _cover = temp;
                    return temp;
                }
                else
                {
                    return _cover;
                }
            }
        }
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