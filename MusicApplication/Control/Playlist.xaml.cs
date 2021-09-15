using Engine;
using Engine.Commands;
using Engine.Model;
using MusicApplication.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MusicApplication.Control
{
    /// <summary>
    /// Interaction logic for Playlist.xaml
    /// </summary>
    public partial class Playlist : UserControl
    {
        public Playlist()
        {
            InitializeComponent();
            PlaylistManager.PlaylistCurrentFileChanged += async () =>
            {
                listView.ScrollIntoView(listView.SelectedItem);
                await Task.Delay(0);
            };
            PlaylistManager.Playlists[0].PlaylistUpdated += async () =>
            {
                listView.Focus();
                await Task.Delay(0);
            };
        }

        private void PlaylistItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PlaylistItem playlistItem = (PlaylistItem)sender;
            MainCommands.Source = playlistItem.FilePath;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listView.ScrollIntoView(listView.SelectedItem);
        }

        private void Item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                StackPanel item = (StackPanel)sender;
                MainCommands.Source = PlaylistManager.PlaylistItems[listView.SelectedIndex].FilePath;
            }
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItem != null)
            {
                foreach (object item in listView.SelectedItems)
                {
                    int index = listView.Items.IndexOf(item);
                    PlaylistManager.Remove(0, index);
                }
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            PlaylistManager.Clear();//=>playlist.clearandnotify
        }

        private async void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            string[] files = await Helper.Utility.FileOpenPicker.GetFileAsync();

            await PlaylistManager.AddRangeAsync(0, files);
        }
    }

    public class PlaylistViewModel : ViewModelBase
    {
        //public ObservableCollection<AudioFile> Playlist { get; set; }
        public ObservableCollection<AudioFile> Playlist { get; set; } = new();
        public PlaylistViewModel()
        {
            PlaylistManager.Playlists[0].PlaylistUpdated += async () =>
            {

                await Task.Run(() =>
                {
                    Playlist = new(PlaylistManager.PlaylistItems);
                    NotifyPropertyChanged(nameof(Playlist));
                });
            };

            PlaylistManager.PlaylistCurrentFileChanged += async () =>
            {
                await Task.Run(() =>
                {
                    SelectedIndex = PlaylistManager.OpenedFileIndex;
                    NotifyPropertyChanged(nameof(SelectedIndex));
                });
            };
        }

        public int SelectedIndex { get; set; }



    }
}
