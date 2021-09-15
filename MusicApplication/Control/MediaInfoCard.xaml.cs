using MusicApplication.ViewModel.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Engine;
using Engine.Model;
using Engine.Commands;
using Engine.Events;
using System.Windows.Media.Imaging;

namespace MusicApplication.Control
{
    /// <summary>
    /// Interaction logic for MediaInfoCard.xaml
    /// </summary>
    public partial class MediaInfoCard : UserControl
    {
        public MediaInfoCard()
        {
            InitializeComponent();
        }
    }

    public class MediaInfoCardViewModel : ViewModelBase
    {
        public ICommand OpenCurrentFileLocationCommand { get; }
        public ICommand SelectCurrentFileInPlaylistCommand { get; }

        public MediaInfoCardViewModel()
        {
            OpenCurrentFileLocationCommand = new DelegateCommand(() => Statics.OpenCurrentFileLocation());
            SelectCurrentFileInPlaylistCommand = new DelegateCommand(() => MainCommands.FindCurrentFile());
            AllEvents.CurrentTimeChanged += AudioPlayer_CurrentTimeChanged;
            AllEvents.PlaybackStateChanged += async (_) =>
            {
                await Task.Run(() =>
                {
                    NotifyPropertyChanged(nameof(TagFile));

                    TagFile = PlaylistManager.PlaylistItems[PlaylistManager.OpenedFileIndex];
                    NotifyPropertyChanged(nameof(Cover));

                    NotifyPropertyChanged(nameof(TotalTimeString));
                    NotifyPropertyChanged(nameof(CurrentTimeString));
                });
            };
        }

        private async Task AudioPlayer_CurrentTimeChanged(TimeSpan Time)
        {
            await Task.Run(() =>
            {
                NotifyPropertyChanged(nameof(CurrentTimeString));
            });
        }

#pragma warning disable CA1822 // Mark members as static
        public AudioFile TagFile { get; set; }
        public BitmapImage Cover => Engine.Utility.Class1.ExtractCover(MainCommands.Source);
        public string CurrentTimeString => MainCommands.CurrentTimeString;
        public string TotalTimeString => MainCommands.TotalTimeString;
#pragma warning restore CA1822 // Mark members as static
    }

}
