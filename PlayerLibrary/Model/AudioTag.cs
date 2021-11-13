using ATL;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PlayerLibrary.Model
{
    //TODO: Make all Property {get; set;}
    public class AudioTag
    {
        public string FilePath { get; set; } = "FilePath";

        private Track tag;

        public AudioTag(string filePath)
        {
            FilePath = filePath;
        }

        private Track Tag => tag ??= new(FilePath);

        public string FileName { get => System.IO.Path.GetFileName(FilePath); }// = "";
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
        public string Bitrate => $"{Tag.Bitrate}kbps";
        public string SampleRate => $"{Tag.SampleRate}Hz";
        public string AudioFormatName => Tag.AudioFormat.Name;

        private BitmapImage _albumArt;
        public BitmapImage AlbumArt
        {
            get
            {
                if (_albumArt == null)
                {
                    BitmapImage img = Utility.CoverImage.ExtractCover(FilePath);
                    _albumArt = img;
                    return img;
                }
                else
                {
                    return _albumArt;
                }
            }
        }

        public Task<BitmapImage> AlbumArtAsync
        {
            get
            {
                if (_albumArt == null)
                {
                    _albumArt = Utility.CoverImage.ExtractCover(FilePath);
                }
                return Utility.CoverImage.AlbumArtAsync(FilePath);
            }
        }
    }
}