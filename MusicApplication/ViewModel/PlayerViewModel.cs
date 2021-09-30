using AudioPlayer;
using AudioPlayer.Model;
using MusicApplication.ViewModel.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

#pragma warning disable CA1822 // Mark members as static
namespace MusicApplication.ViewModel
{
    public class PlayerViewModel : ViewModelBase
    {
        private Player Player = SharedStatics.Player;

        public ICommand OpenCurrentFileLocationCommand { get; }
        public ICommand SelectCurrentFileInPlaylistCommand { get; }
        public ICommand PlayPauseCommand { get; }

        private DelegateCommand _nextAudioCommand;
        public ICommand NextAudioCommand => _nextAudioCommand ??= new DelegateCommand(NextAudio);

        private DelegateCommand _previousAudioFileCommand;
        public ICommand PreviousAudioFileCommand => _previousAudioFileCommand ??= new DelegateCommand(PreviousAudioFile);

        public PlayerViewModel()
        {
            PlayPauseCommand = new DelegateCommand(() => PlayPause());

            Player.VolumeChanged += AudioPlayer_VolumeChanged;
            NotifyPropertyChanged(null);
            OpenCurrentFileLocationCommand = new DelegateCommand(() => SharedStatics.OpenCurrentFileLocation());
            Player.CurrentTimeChanged += AudioPlayer_CurrentTimeChanged;
            Player.PlaybackStateChanged += Player_PlaybackStateChanged;

            NotifyPropertyChanged(null);
        }

        private async void Player_PlaybackStateChanged(PlaybackState newPlaybackState)
        {
            IsPlaying = newPlaybackState == PlaybackState.Playing;
            NotifyPropertyChanged(nameof(IsPlaying));

            await Task.Run(() =>
            {
                NotifyPropertyChanged(nameof(TotalTimeString));
                NotifyPropertyChanged(nameof(CurrentTimeString));
                NotifyPropertyChanged(nameof(TotalTimeTotalSeconds));
                NotifyPropertyChanged(nameof(CurrentTimeTotalSeconds));
            });

            await Task.Run(() =>
            {
                if (newPlaybackState is PlaybackState.Opened)
                {
                    TagFile = new() { FilePath = Player.Source };
                    NotifyPropertyChanged(nameof(TagFile));

                    NotifyPropertyChanged(nameof(TotalTimeString));
                    NotifyPropertyChanged(nameof(CurrentTimeString));

                    AudioPlayer.Utility.CoverImage2 CoverImage2 = new();
                    CoverImage2.OnImageCreated += (BitmapImage ti) => { Cover = ti; NotifyPropertyChanged(nameof(Cover)); };
                    CoverImage2.CreateImage(Player.Source);
                }
            });
        }

        public BitmapImage Cover { get; set; }

        public AudioFile TagFile { get; set; }

        private void PlayPause()
        {
            if (IsPlaying)
            {
                Player.Pause();
            }
            else if (Player.PlaybackState is not PlaybackState.Closed)
            {
                Player.Play();
            }
            else
            {
                SharedStatics.OpenFilePicker();
            }
        }

        private async void AudioPlayer_VolumeChanged(int newVolume)
        {
            await Task.Run(() => { Volume = newVolume; NotifyPropertyChanged(nameof(Volume)); });
        }

        private async void AudioPlayer_CurrentTimeChanged(TimeSpan Time)
        {
            await Task.Run(() =>
            {
                int ts = (int)Time.TotalSeconds;
                if (CurrentTimeTotalSeconds != ts)
                {
                    CurrentTimeTotalSeconds = ts;
                    NotifyPropertyChanged(nameof(CurrentTimeTotalSeconds));
                    NotifyPropertyChanged(nameof(CurrentTimeString));
                }
            });
        }

        public string CurrentTimeString => Player.TimePosition.ToString(SharedStatics.stringformat);

        public string TotalTimeString => Player.TimeDuration.ToString(SharedStatics.stringformat);

        public int CurrentTimeTotalSeconds { get; set; }

        public double TotalTimeTotalSeconds => Player.TimeDuration.TotalSeconds;

        public double Volume { get; set; } = SharedStatics.Player.Volume;

        public bool IsPlaying { get; set; } = SharedStatics.Player.PlaybackState == PlaybackState.Playing;

        private void NextAudio()
        {
            //Player.PlaylistManager.PlayNext();
        }

        private void PreviousAudioFile()
        {
            //Player.PlaylistManager.PlayPrevious();
        }
    }
}

#pragma warning restore CA1822 // Mark members as static