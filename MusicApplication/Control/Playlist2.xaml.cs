using Engine;
using Engine.Model;
using MusicApplication.ViewModel.Base;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MusicApplication.Control
{
    /// <summary>
    /// Interaction logic for Playlist.xaml
    /// </summary>
    public partial class Playlist2 : UserControl
    {
        PlaylistManager PlaylistManager = Shared.Player.PlaylistManager;
        Player Player = Shared.Player;

        public Playlist2()
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

    public class PlaylistViewModel2 : ViewModelBase
    {
        //public ObservableCollection<AudioFile> Playlist { get; set; }
        public ObservableCollection<AudioFile> Playlist { get; set; } = new();
        public PlaylistViewModel2()
        {
            NotifyPropertyChanged(null);
           // Shared.Player.PlaylistManager.Playlists[0].PlaylistUpdated += PlaylistViewModel_PlaylistUpdated;
        }

        private async void PlaylistViewModel_PlaylistUpdated()
        {
            await Task.Run(() =>
            {
                Playlist = new(Shared.Player.PlaylistManager.PlaylistItems.ToArray());
                NotifyPropertyChanged(nameof(Playlist));
            });
        }




    }
}
