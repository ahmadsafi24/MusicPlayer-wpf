using PlayerLibrary;
using PlayerLibrary.Model;
using PlayerUI.ViewModel.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PlayerUI.ViewModel
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

        PlayerLibrary.Utility.CoverImage2 CoverImage2 = new();
        public PlayerViewModel()
        {
            PlayPauseCommand = new DelegateCommand(() => PlayPause());
            MuteAudioCommand = new DelegateCommand(() => MuteUnmute());

            Player.VolumeChanged += AudioPlayer_VolumeChanged;
            Player.TimePositionChanged += AudioPlayer_CurrentTimeChanged;
            Player.PlaybackStateChanged += Player_PlaybackStateChanged;
            CoverImage2.OnImageCreated += (BitmapImage ti) => { Cover = ti; NotifyPropertyChanged(nameof(Cover)); };
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
                NotifyPropertyChanged(nameof(TotalTime));
                NotifyPropertyChanged(nameof(CurrentTime));
            });

            if (newPlaybackState== PlaybackState.Paused)
            {
                Helper.Taskbar.Progress.SetState(Helper.Taskbar.ProgressState.Paused,true);
            }
            else
            {
                Helper.Taskbar.Progress.SetState(Helper.Taskbar.ProgressState.Normal,true);
            }

            if (newPlaybackState is PlaybackState.Opened)
            {
                TagFile = new() { FilePath = Player.Source };
                CoverImage2.CreateImage(Player.Source);
                NotifyPropertyChanged(nameof(TagFile));
                NotifyPropertyChanged(nameof(TotalTime));
                NotifyPropertyChanged(nameof(CurrentTime));
                NotifyPropertyChanged(nameof(CoreCurrentFileInfo));
                Player.Play();
            }
            if (newPlaybackState == PlaybackState.Ended)
            {
                Player.Close();
            }
            if (newPlaybackState == PlaybackState.Closed)
            {
                TagFile = new() { FilePath = Player.Source };
                Cover = null;
                NotifyPropertyChanged(nameof(Cover));
                NotifyPropertyChanged(nameof(TagFile));
                NotifyPropertyChanged(nameof(TotalTime));
                NotifyPropertyChanged(nameof(CurrentTime));
                NotifyPropertyChanged(nameof(CoreCurrentFileInfo));
                AudioPlayer_CurrentTimeChanged(TimeSpan.Zero);
            }
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
                    await Player.SeekAsync(TimeSpan.Zero);
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
            await Task.Run(() => { _volume = newVolume; NotifyPropertyChanged(nameof(Volume)); NotifyPropertyChanged(nameof(IsMuted)); });
        }

        private async void AudioPlayer_CurrentTimeChanged(TimeSpan Time)
        {
            await Task.Run(() =>
            {
                _currentTime = Time;
                NotifyPropertyChanged(nameof(CurrentTime));
                setprogressvalue(Time, TotalTime);
            });
        }

        private static int lastValue = 0;

        private static void settaskvalue(int val)
        {
            if (val != lastValue)
            {
                Helper.Taskbar.Progress.SetValue(val, 100, true);
            }

            lastValue = val;
        }
        private static void setprogressvalue(TimeSpan currentTime, TimeSpan maxTime)
        {
            if (maxTime <= TimeSpan.FromSeconds(10))
            {
                settaskvalue(0);
                return;
            }
            int percentValue = (int)maxTime.TotalSeconds / 100;
            int currentValue = (int)currentTime.TotalSeconds / percentValue;
            settaskvalue(currentValue);
        }

        public TimeSpan TotalTime => Player.TimeDuration;

        private TimeSpan _currentTime;
        public TimeSpan CurrentTime
        {
            get => _currentTime;
            set => Task.Run(async () => await Player.SeekAsync(value));
        }

        private double _volume = App.Player.Volume;
        public double Volume
        {
            get => _volume;
            set => Player.ChangeVolume((int)value);
        }

        public bool IsPlaying { get; set; } = App.Player.PlaybackState == PlaybackState.Playing;

        private void NextAudio()
        {
            //Player.PlaylistManager.PlayNext();
        }

        private void PreviousAudioFile()
        {

        }

        public string CoreCurrentFileInfo
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(Player.Source))
                    {
                        return $"{Player.ReaderInfo.SampleRate} {Player.ReaderInfo.BitsPerSample}bit {Player.ReaderInfo.Encoding}";
                    }
                    return "";
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }
    }
}