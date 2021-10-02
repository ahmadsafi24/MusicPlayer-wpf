using AudioPlayer;
using AudioPlayer.Model;
using MusicApplication.ViewModel.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MusicApplication.ViewModel
{
    public class PlayerViewModel : ViewModelBase
    {
        private readonly Player Player = App.Player;

        public ICommand PlayPauseCommand { get; }
        public ICommand MuteAudioCommand { get; }

        private DelegateCommand _nextAudioCommand;
        public ICommand NextAudioCommand => _nextAudioCommand ??= new DelegateCommand(NextAudio);

        private DelegateCommand _previousAudioFileCommand;
        public ICommand PreviousAudioFileCommand => _previousAudioFileCommand ??= new DelegateCommand(PreviousAudioFile);

        public PlayerViewModel()
        {
            PlayPauseCommand = new DelegateCommand(() => PlayPause());
            MuteAudioCommand = new DelegateCommand(() => MuteUnmute());

            Player.VolumeChanged += AudioPlayer_VolumeChanged;
            NotifyPropertyChanged(null);
            Player.TimePositionChanged += AudioPlayer_CurrentTimeChanged;
            Player.PlaybackStateChanged += Player_PlaybackStateChanged;

            NotifyPropertyChanged(null);
        }

        public bool IsMuted { get => Player.IsMuted; set => Player.IsMuted = value; }

        private void MuteUnmute()
        {
            IsMuted = !IsMuted;
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

        private async void PlayPause()
        {
            switch (Player.PlaybackState)
            {
                case PlaybackState.Playing:
                    Player.Pause();
                    break;
                case PlaybackState.Ended:
                    await Player.SeekAsync(0);
                    Player.Play();
                    break;
                case PlaybackState.Closed:
                    Commands.FilePicker.OpenFilePicker();
                    break;
                default:
                    Player.Play();
                    break;
            }
        }

        private async void AudioPlayer_VolumeChanged(int newVolume)
        {
            await Task.Run(() => { Volume = newVolume; NotifyPropertyChanged(nameof(Volume)); NotifyPropertyChanged(nameof(IsMuted)); });
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

        public string CurrentTimeString => Converter.TimeSpan.ToString(Player.TimePosition);

        public string TotalTimeString => Converter.TimeSpan.ToString(Player.TimeDuration);

        public int CurrentTimeTotalSeconds { get; set; }

        public double TotalTimeTotalSeconds => Player.TimeDuration.TotalSeconds;

        public double Volume { get; set; } = App.Player.Volume;

        public bool IsPlaying { get; set; } = App.Player.PlaybackState == PlaybackState.Playing;

        private void NextAudio()
        {
            //Player.PlaylistManager.PlayNext();
        }

        private void PreviousAudioFile()
        {
            Player.Playlist.PlayPrevious();
        }
    }
}