using ATL;
using System.Windows.Media.Imaging;

namespace PlayerLibrary.Model
{
    public struct AudioFile
    {
        public string FilePath;

        private Track tag;
        private Track Tag => tag ??= new(FilePath);

        public string FileName => System.IO.Path.GetFileName(FilePath);
        public string Title => string.IsNullOrEmpty(Tag.Title) ? FileName : Tag.Title;
        public string Artist => string.IsNullOrEmpty(Tag.Artist) ? AlbumArtist : Tag.Artist;
        public string Album
        {
            get
            {
                if (Tag == null)
                {
                    return null;
                }
                string val;
                if (string.IsNullOrEmpty(Tag.Album))
                { val = null; return val; }
                else
                {
                    val = Tag.Album;
                }
                if (val.Equals(Title, System.StringComparison.OrdinalIgnoreCase) ||
                    val.Equals(Artist, System.StringComparison.OrdinalIgnoreCase))
                { val = null; }
                return val;
            }
        }

        public string AlbumArtist => string.IsNullOrEmpty(Tag.AlbumArtist) ? null : Tag.AlbumArtist;
        public BitmapImage Cover => Utility.CoverImage.ExtractCover(FilePath);

        //public string FileNameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(FilePath);
        //public string FileExtension => System.IO.Path.GetExtension(FilePath);
        //public string FileDirectory => System.IO.Path.GetDirectoryName(FilePath);
        /*private System.Threading.Tasks.Task<BitmapImage> _cover;
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
        }*/
    }

}