using ATL;
using System.Windows.Media.Imaging;

namespace AudioPlayer.Model
{
    public struct AudioFile
    {
        public string FilePath;

        private Track tag;
        private Track Tag => tag ??= new(FilePath);




        public string FileName => System.IO.Path.GetFileName(FilePath);
        //public string FileNameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(FilePath);
        //public string FileExtension => System.IO.Path.GetExtension(FilePath);
        //public string FileDirectory => System.IO.Path.GetDirectoryName(FilePath);
        public string Title => Tag.Title;
        public string Artist => Tag.Artist;
        public string Album => Tag.Album;
        public string AlbumArtist => Tag.AlbumArtist;

        public BitmapImage Cover => Utility.CoverImage.ExtractCover(FilePath);

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