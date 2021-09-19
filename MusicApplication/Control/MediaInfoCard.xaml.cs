using Engine;
using Engine.Model;
using MusicApplication.ViewModel.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MusicApplication.Control
{
    /// <summary>
    /// Interaction logic for MediaInfoCard.xaml
    /// </summary>
    public partial class MediaInfoCard : UserControl
    {
        public MediaInfoCard() => InitializeComponent();
    }

    public class MediaInfoCardViewModel : ViewModelBase
    {
        public ICommand OpenCurrentFileLocationCommand { get; }
        public ICommand SelectCurrentFileInPlaylistCommand { get; }

        public MediaInfoCardViewModel()
        {
            OpenCurrentFileLocationCommand = new DelegateCommand(() => Shared.OpenCurrentFileLocation());
            SelectCurrentFileInPlaylistCommand = new DelegateCommand(() => Player.FindCurrentFile());
            Player.CurrentTimeChanged += AudioPlayer_CurrentTimeChanged;
            Player.PlaybackStateChanged += async (Engine.Enums.PlaybackState playbackState) =>
            {
                if (playbackState == Engine.Enums.PlaybackState.Opened)
                {
                    await Task.Run(() =>
                    {
                        TagFile = new() { FilePath = Player.Source };
                        NotifyPropertyChanged(nameof(TagFile));

                        NotifyPropertyChanged(nameof(TotalTimeString));
                        NotifyPropertyChanged(nameof(CurrentTimeString));

                        Engine.Utility.CoverImage2 CoverImage2 = new();
                        CoverImage2.OnImageCreated += (BitmapImage ti) => { Cover = ti; NotifyPropertyChanged(nameof(Cover)); };
                        CoverImage2.CreateImage(Player.Source);
                    });
                }
            };
        }

        public BitmapImage Cover { get; set; }

        private async void AudioPlayer_CurrentTimeChanged(TimeSpan Time)
        {
            await Task.Run(() =>
            {
                NotifyPropertyChanged(nameof(CurrentTimeString));
            });
        }

#pragma warning disable CA1822 // Mark members as static
        public AudioFile TagFile { get; set; }

        public string CurrentTimeString => Player.CurrentTime.ToString(Shared.stringformat);
        public string TotalTimeString => Player.TotalTime.ToString(Shared.stringformat);
#pragma warning restore CA1822 // Mark members as static
    }

}
