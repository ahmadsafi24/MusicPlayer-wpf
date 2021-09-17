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
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listView.ScrollIntoView(listView.SelectedItem);
        }

        private async void Item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                MainCommands.Source = PlaylistManager.PlaylistItems[listView.SelectedIndex].FilePath;
                await MainCommands.OpenAsync();
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

        private async Task PlaylistViewModel_PlaylistUpdated()
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
