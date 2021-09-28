using Engine;
using Engine.Model;
using MusicApplication.ViewModel.Base;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
namespace MusicApplication.Control
{
    /// <summary>
    /// Interaction logic for Playlist.xaml
    /// </summary>
    public partial class Playlist : UserControl
    {
        Player Player = Shared.Player;
        PlaylistManager PlaylistManager = Shared.Player.PlaylistManager;
        public Playlist()
        {
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listView.ScrollIntoView(listView.SelectedItem);
        }

        private async void Item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                Player.Source = PlaylistManager.PlaylistItems[listView.SelectedIndex].FilePath;
                await Player.OpenAsync();
            }
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedIndex is not -1)
            {
                int index = listView.SelectedIndex;
                PlaylistManager.Remove(0, index);

            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            PlaylistManager.Clear();//=>playlist.clearandnotify
        }

        private async void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            string[] files = await Helper.FileOpenPicker.GetFileAsync();

            await PlaylistManager.AddRangeAsync(0, files);
        }
    }

    public class PlaylistViewModel : ViewModelBase
    {
        Player Player = Shared.Player;
        PlaylistManager PlaylistManager = Shared.Player.PlaylistManager;
        //public ObservableCollection<AudioFile> Playlist { get; set; }
        public ObservableCollection<AudioFile> Playlist { get; set; } = new();
        public PlaylistViewModel()
        {
            NotifyPropertyChanged(null);
            PlaylistManager.Playlists[0].PlaylistUpdated += PlaylistViewModel_PlaylistUpdated;

            PlaylistManager.PlaylistCurrentFileChanged += async () =>
            {
                await Task.Run(() =>
                {
                    SelectedIndex = PlaylistManager.OpenedFileIndex;
                    NotifyPropertyChanged(nameof(SelectedIndex));
                });
            };
        }

        private async void PlaylistViewModel_PlaylistUpdated()
        {
            await Task.Run(() =>
            {
                Playlist = new(PlaylistManager.PlaylistItems);
                NotifyPropertyChanged(nameof(Playlist));
            });
        }

        public int SelectedIndex { get; set; } = -1;



    }
}
