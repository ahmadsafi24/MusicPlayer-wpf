using PlayerLibrary;
using PlayerLibrary.Model;
using PlayerUI.ViewModel.Base;
using System;
using System.IO;
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
        public ICommand OpenCoverFileCommand { get; }

        private DelegateCommand _nextAudioCommand;
        public ICommand NextAudioCommand => _nextAudioCommand ??= new DelegateCommand(NextAudio);

        private DelegateCommand _previousAudioFileCommand;
        public ICommand PreviousAudioFileCommand => _previousAudioFileCommand ??= new DelegateCommand(PreviousAudioFile);

        PlayerLibrary.Utility.CoverImage2 CoverImage2 = new();
        public PlayerViewModel()
        {
            PlayPauseCommand = new DelegateCommand(PlayPause);
            MuteAudioCommand = new DelegateCommand(MuteUnmute);
            OpenCoverFileCommand = new DelegateCommand(OpenCoverFile);

            Player.VolumeChanged += AudioPlayer_VolumeChanged;
            Player.TimePositionChanged += AudioPlayer_CurrentTimeChanged;
            Player.PlaybackStateChanged += Player_PlaybackStateChanged;
            CoverImage2.OnImageCreated += (BitmapImage ti) => { Cover = ti; NotifyPropertyChanged(nameof(Cover)); };
        }

        private void OpenCoverFile()
        {
            if (Cover == null) return;
            string ImageFilePath = @"C:\Users\ahmad\Desktop\test.png";
            Helper.File.SaveBitmapImageToPng(Cover, ImageFilePath);
            Helper.File.OpenFileWithDefaultApp(ImageFilePath);
        }

        public bool IsMuted { get => Player.IsMuted; set => Player.IsMuted = value; }

        private void MuteUnmute()
        {
            IsMuted = !IsMuted;
        }

        private void UpdateTotalTime() => NotifyPropertyChanged(nameof(TotalTime));
        private void UpdateCurrentTime() => NotifyPropertyChanged(nameof(CurrentTime));
        private async void Player_PlaybackStateChanged(PlaybackState playbackState)
        {
            IsPlaying = playbackState == PlaybackState.Playing;
            NotifyPropertyChanged(nameof(IsPlaying));
            await Task.Run(() =>
            {
                UpdateTotalTime();
                UpdateCurrentTime();
            });
            switch (playbackState)
            {
                case PlaybackState.Unknown:
                    break;
                case PlaybackState.Failed:
                    break;
                case PlaybackState.Opened:
                    TagFile = new() { FilePath = Player.Source };
                    CoverImage2.CreateImage(Player.Source);
                    UpdateAll();
                    Player.Play();
                    break;
                //if
                case PlaybackState.Paused:
                    Commands.Taskbar.SetTaskbarState(Helper.Taskbar.ProgressState.Paused);
                    return;
                case PlaybackState.Playing:
                    Commands.Taskbar.SetTaskbarState(Helper.Taskbar.ProgressState.Normal);
                    break;
                case PlaybackState.Stopped:
                    Commands.Taskbar.SetTaskbarState(Helper.Taskbar.ProgressState.Normal);
                    break;
                case PlaybackState.Ended:
                    await Player.OpenAsync(Player.Source);
                    break;
                case PlaybackState.Closed:
                    TagFile = new() { FilePath = Player.Source };
                    Cover = null;
                    UpdateAll();
                    break;
                default:
                    break;
            }

            //else

        }

        private void UpdateAll()
        {
            NotifyPropertyChanged(nameof(Cover));
            NotifyPropertyChanged(nameof(TagFile));
            UpdateTotalTime();
            UpdateCurrentTime();
            NotifyPropertyChanged(nameof(CoreCurrentFileInfo));
            AudioPlayer_CurrentTimeChanged(TimeSpan.Zero);
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
                UpdateCurrentTime();
                Commands.Taskbar.SetTaskbarProgressValue(Time, TotalTime);
            });
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