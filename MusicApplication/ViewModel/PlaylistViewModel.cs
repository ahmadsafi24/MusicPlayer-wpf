using AudioPlayer;
using AudioPlayer.Model;
using MusicApplication.ViewModel.Base;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MusicApplication.ViewModel
{
    public class PlaylistViewModel : ViewModelBase
    {
        private readonly PlaylistV2 PlaylistV2 = App.Player.Playlist;
        //public ObservableCollection<AudioFile> Playlist { get; set; }
        public ObservableCollection<AudioFile> Playlist { get; set; } = new();
        public PlaylistViewModel()
        {
            NotifyPropertyChanged(null);
            PlaylistV2.Updated += PlaylistViewModel_PlaylistUpdated;

            /*PlaylistManager.PlaylistCurrentFileChanged += async () =>
            {
                await Task.Run(() =>
                {
                    SelectedIndex = PlaylistManager.OpenedFileIndex;
                    NotifyPropertyChanged(nameof(SelectedIndex));
                });
            };*/
        }

        private async void PlaylistViewModel_PlaylistUpdated()
        {
            await Task.Run(() =>
            {
                var tc = new PlaylistFile(PlaylistV2.Pathlist);
                Playlist = new(tc.Items);
                NotifyPropertyChanged(nameof(Playlist));
            });
        }

        public int SelectedIndex { get; set; } = -1;



    }

}
