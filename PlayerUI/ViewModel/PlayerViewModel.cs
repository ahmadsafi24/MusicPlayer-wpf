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

        public PlayerViewModel()
        {
            PlayPauseCommand = new DelegateCommand(PlayPause);
            MuteAudioCommand = new DelegateCommand(MuteUnmute);
            OpenCoverFileCommand = new DelegateCommand(OpenCoverFile);

            NextAudioCommand = new DelegateCommand(NextAudio);
            PreviousAudioCommand = new DelegateCommand(PreviousAudio);

            Player.VolumeController.VolumeChanged += AudioPlayer_VolumeChanged;
            Player.PlaybackSession.TimelineController.TimePositionChanged += AudioPlayer_CurrentTimeChanged;
            Player.PlaybackSession.PlaybackStateChanged += Player_PlaybackStateChanged;
        }

        private void OpenCoverFile()
        {
            if (Cover == null) return;
            string ImageFilePath = AppContext.BaseDirectory + @"\temp_cover.png";
            Helper.File.SaveBitmapImageToPng(Cover, ImageFilePath);
            Helper.File.OpenFileWithDefaultApp(ImageFilePath);
        }

        public bool IsMuted { get => Player.VolumeController.IsMuted; set => Player.VolumeController.IsMuted = value; }

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
                    TagFile = new() { FilePath = Player.PlaybackSession.AudioFilePath };
                    //Cover =  await CoverImage.AlbumArtAsync(Player.Controller.AudioFilePath);
                    UpdateAll();
                    Player.PlaybackSession.Play();
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
                    await Player.PlaybackSession.OpenAsync(Player.PlaybackSession.AudioFilePath);
                    break;
                case PlaybackState.Closed:
                    TagFile = new() { FilePath = Player.PlaybackSession.AudioFilePath };
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
            switch (Player.PlaybackSession.PlaybackState)
            {
                case PlaybackState.Playing:
                    Player.PlaybackSession.Pause();
                    break;
                case PlaybackState.Ended:
                    await Player.PlaybackSession.TimelineController.SeekAsync(TimeSpan.Zero);
                    Player.PlaybackSession.Play();
                    break;
                case PlaybackState.Closed:
                    Commands.FilePicker.OpenFilePicker(Player);
                    break;
                default:
                    Player.PlaybackSession.Play();
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


        public TimeSpan TotalTime => Player.PlaybackSession.TimelineController.Total;

        private TimeSpan _currentTime;
        public TimeSpan CurrentTime
        {
            get => _currentTime;
            set => Task.Run(async () => await Player.PlaybackSession.TimelineController.SeekAsync(value));
        }

        private double _volume = App.Player.VolumeController.Volume;
        public double Volume
        {
            get => _volume;
            set => Player.VolumeController.ChangeVolume((int)value);
        }

        public bool IsPlaying { get; set; } = App.Player.PlaybackSession.PlaybackState == PlaybackState.Playing;

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
                    if (!string.IsNullOrEmpty(Player.PlaybackSession.AudioFilePath))
                    {
                        if (Player.PlaybackSession.AudioInfo == null) return "empty";
                        AudioInfo audioInfo = Player.PlaybackSession.AudioInfo;
                        return $"{audioInfo.Format} {audioInfo.BitrateString} {audioInfo.SampleRate / 1000}kHz {audioInfo.Channels}ch";
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