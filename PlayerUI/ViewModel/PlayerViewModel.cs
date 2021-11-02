using Helper.ViewModelBase;
using PlayerLibrary;
using PlayerLibrary.FileInfo;
using PlayerLibrary.Model;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PlayerUI.ViewModel
{
    public class PlayerViewModel : ViewModelBase
    {
        private Player Player => App.Player;
        public ICommand PlayPauseCommand { get; }
        public ICommand MuteAudioCommand { get; }
        public ICommand OpenCoverFileCommand { get; }
        public ICommand NextAudioCommand { get; }
        public ICommand PreviousAudioCommand { get; }

        PlayerLibrary.Utility.CoverImage2 CoverImage2 = new();
        public PlayerViewModel()
        {
            PlayPauseCommand = new DelegateCommand(PlayPause);
            MuteAudioCommand = new DelegateCommand(MuteUnmute);
            OpenCoverFileCommand = new DelegateCommand(OpenCoverFile);

            NextAudioCommand = new DelegateCommand(NextAudio);
            PreviousAudioCommand = new DelegateCommand(PreviousAudio);

            Player.VolumeChanged += AudioPlayer_VolumeChanged;
            Player.Timing.TimePositionChanged += AudioPlayer_CurrentTimeChanged;
            Player.PlaybackStateChanged += Player_PlaybackStateChanged;
            CoverImage2.OnImageCreated += (BitmapImage ti) => { Cover = ti; NotifyPropertyChanged(nameof(Cover)); };
        }

        private void OpenCoverFile()
        {
            if (Cover == null) return;
            string ImageFilePath = AppContext.BaseDirectory + @"\temp_cover.png";
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
            CurrentPlaybackState = playbackState.ToString();
            NotifyPropertyChanged(nameof(CurrentPlaybackState));

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
                    TagFile = new() { FilePath = Player.Controller.AudioFilePath };
                    CoverImage2.CreateImage(Player.Controller.AudioFilePath);
                    UpdateAll();
                    Player.Controller.Play();
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
                    await Player.Controller.OpenAsync(Player.Controller.AudioFilePath);
                    break;
                case PlaybackState.Closed:
                    TagFile = new() { FilePath = Player.Controller.AudioFilePath };
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
            AudioPlayer_CurrentTimeChanged(TimeSpan.Zero);

            NotifyPropertyChanged(nameof(CoreCurrentFileInfo));
        }
        public BitmapImage Cover { get; set; }

        public AudioTag TagFile { get; set; }

        private async void PlayPause()
        {
            switch (Player.Controller.PlaybackState)
            {
                case PlaybackState.Playing:
                    Player.Controller.Pause();
                    break;
                case PlaybackState.Ended:
                    await Player.Timing.SeekAsync(TimeSpan.Zero);
                    Player.Controller.Play();
                    break;
                case PlaybackState.Closed:
                    Commands.FilePicker.OpenFilePicker(Player);
                    break;
                default:
                    Player.Controller.Play();
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


        public TimeSpan TotalTime => Player.Timing.Total;

        private TimeSpan _currentTime;
        public TimeSpan CurrentTime
        {
            get => _currentTime;
            set => Task.Run(async () => await Player.Timing.SeekAsync(value));
        }

        private double _volume = App.Player.Volume;
        public double Volume
        {
            get => _volume;
            set => Player.ChangeVolume((int)value);
        }

        public bool IsPlaying { get; set; } = App.Player.Controller.PlaybackState == PlaybackState.Playing;

        private void NextAudio()
        {
            //Player.PlaylistManager.PlayNext();
        }

        private void PreviousAudio()
        {

        }


        public string CoreCurrentFileInfo
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(Player.Controller.AudioFilePath))
                    {
                        if (Player.Controller.AudioInfo == null) return "empty";
                        AudioInfo audioInfo = Player.Controller.AudioInfo;
                        return $"{audioInfo.Format} {audioInfo.BitrateString} {audioInfo.SampleRate/1000}kHz {audioInfo.Channels}ch";
                    }
                    return "";
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }
        public string CurrentPlaybackState { get; set; }
    }
}