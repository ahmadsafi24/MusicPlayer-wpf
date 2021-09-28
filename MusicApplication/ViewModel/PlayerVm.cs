using Engine;
using Engine.Enums;
using Engine.Model;
using MusicApplication.ViewModel.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

#pragma warning disable CA1822 // Mark members as static
namespace MusicApplication.ViewModel
{
    public class PlayerVm : ViewModelBase
    {
        public ICommand OpenCurrentFileLocationCommand { get; }
        public ICommand SelectCurrentFileInPlaylistCommand { get; }
        public ICommand PlayPauseCommand { get; }

        private DelegateCommand _nextAudioCommand;
        public ICommand NextAudioCommand => _nextAudioCommand ??= new DelegateCommand(NextAudio);

        private DelegateCommand _previousAudioFileCommand;
        public ICommand PreviousAudioFileCommand => _previousAudioFileCommand ??= new DelegateCommand(PreviousAudioFile);

        public PlayerVm()
        {
            PlayPauseCommand = new DelegateCommand(() => PlayPause());

            Shared.Player.VolumeChanged += AudioPlayer_VolumeChanged;
            NotifyPropertyChanged(null);
            OpenCurrentFileLocationCommand = new DelegateCommand(() => Shared.OpenCurrentFileLocation());
            Shared.Player.CurrentTimeChanged += AudioPlayer_CurrentTimeChanged;
            Shared.Player.PlaybackStateChanged += Player_PlaybackStateChanged;

            NotifyPropertyChanged(null);
        }

        private async void Player_PlaybackStateChanged(Engine.Enums.PlaybackState newPlaybackState)
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
                if (newPlaybackState is Engine.Enums.PlaybackState.Opened)
                {
                    TagFile = new() { FilePath = Shared.Player.Source };
                    NotifyPropertyChanged(nameof(TagFile));

                    NotifyPropertyChanged(nameof(TotalTimeString));
                    NotifyPropertyChanged(nameof(CurrentTimeString));

                    Engine.Utility.CoverImage2 CoverImage2 = new();
                    CoverImage2.OnImageCreated += (BitmapImage ti) => { Cover = ti; NotifyPropertyChanged(nameof(Cover)); };
                    CoverImage2.CreateImage(Shared.Player.Source);
                }
            });
        }

        public BitmapImage Cover { get; set; }

        public AudioFile TagFile { get; set; }

        private void PlayPause()
        {
            if (IsPlaying)
            {
                Shared.Player.Pause();
            }
            else if (Shared.Player.PlaybackState is not PlaybackState.Closed)
            {
                Shared.Player.Play();
            }
            else
            {
                Shared.OpenFilePicker();
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

        public string CurrentTimeString => Shared.Player.CurrentTime.ToString(Shared.stringformat);

        public string TotalTimeString => Shared.Player.TotalTime.ToString(Shared.stringformat);

        public int CurrentTimeTotalSeconds { get; set; }

        public double TotalTimeTotalSeconds => Shared.Player.TotalTime.TotalSeconds;

        public double Volume { get; set; } = Shared.Player.Volume;

        public bool IsPlaying { get; set; } = Shared.Player.PlaybackState == PlaybackState.Playing;

        private void NextAudio()
        {
            Shared.Player.PlaylistManager.PlayNext();
        }

        private void PreviousAudioFile()
        {
            Shared.Player.PlaylistManager.PlayPrevious();
        }
    }
}

#pragma warning restore CA1822 // Mark members as static