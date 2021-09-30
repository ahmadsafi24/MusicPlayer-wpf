using AudioPlayer;
using AudioPlayer.Model;
using MusicApplication.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicApplication.ViewModel
{
    public class PlaylistViewModel : ViewModelBase
    {
        Player Player = SharedStatics.Player;
        PlaylistV2 PlaylistV2 = SharedStatics.Player.Playlist;
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
                var tc = new PlaylistFile(PlaylistV2.pathlist);
                Playlist = new(tc.Items);
                NotifyPropertyChanged(nameof(Playlist));
            });
        }

        public int SelectedIndex { get; set; } = -1;



    }

}
