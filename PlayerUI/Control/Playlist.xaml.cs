using PlayerLibrary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace PlayerUI.Control
{
    /// <summary>
    /// Interaction logic for Playlist.xaml
    /// </summary>
    public partial class Playlist : UserControl
    {
        private readonly Player Player = App.Player;
        private readonly PlaylistV2 Playlistmanager = App.Player.Playlist;
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
                Player.Source = Playlistmanager.Pathlist[listView.SelectedIndex];
                await Player.OpenAsync();
            }
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedIndex is not -1)
            {
                int index = listView.SelectedIndex;
                Playlistmanager.Pathlist.RemoveAt(index);
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            Playlistmanager.Pathlist.Clear();//=>playlist.clearandnotify
        }

        private async void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            string[] files = await Helper.FileOpenPicker.GetFileAsync();

            await Playlistmanager.AddRangeAsync(files);
        }
    }

}
