using AudioPlayer;
using AudioPlayer.Model;
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
        Player Player = SharedStatics.Player;
        private PlaylistV2 Playlistmanager = SharedStatics.Player.Playlist;
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
                Player.Source = Playlistmanager.pathlist[listView.SelectedIndex];
                await Player.OpenAsync();
            }
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedIndex is not -1)
            {
                int index = listView.SelectedIndex;
                Playlistmanager.pathlist.RemoveAt(index);
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            Playlistmanager.pathlist.Clear();//=>playlist.clearandnotify
        }

        private async void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            string[] files = await Helper.FileOpenPicker.GetFileAsync();

            await Playlistmanager.AddRangeAsync(files);
        }
    }

}
