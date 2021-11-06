using Helper.ViewModelBase;
using PlayerLibrary;
using PlayerLibrary.FileInfo;
using PlayerLibrary.Model;
using PlayerLibrary.Shell;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PlayerUI.ViewModel
{
    public class PlayerViewModel : ViewModelBase
    {
        private SoundPlayer player => App.Player;
        private PlaybackSession playbackSession => App.Player.PlaybackSession;
        private TimelineController timelineController => App.Player.PlaybackSession.TimelineController;
        private VolumeController volumeController => App.Player.PlaybackSession.VolumeController;

        #region Commands
        public ICommand PlayPauseCommand { get; }
        public ICommand MuteAudioCommand { get; }
        public ICommand OpenCoverFileCommand { get; }
        public ICommand NextAudioCommand { get; }
        public ICommand PreviousAudioCommand { get; }

        #endregion

        #region Bindable Property
        public bool IsMuted
        {
            get => volumeController.IsMuted;
            set => volumeController.IsMuted = value;
        }
        public string CurrentPlaybackState => playbackSession.PlaybackState.ToString();
        public BitmapImage Cover => TagFile.AlbumArt;
        public AudioTag TagFile => playbackSession.AudioInfo?.AudioTag;
        public TimeSpan TotalTime => timelineController.Total;

        #endregion

        public PlayerViewModel()
        {
            PlayPauseCommand = new DelegateCommand(PlayPause);
            MuteAudioCommand = new DelegateCommand(MuteUnmute);
            OpenCoverFileCommand = new DelegateCommand(OpenCoverFile);

            NextAudioCommand = new DelegateCommand(NextAudio);
            PreviousAudioCommand = new DelegateCommand(PreviousAudio);

            volumeController.VolumeChanged += AudioPlayer_VolumeChanged;
            timelineController.TimePositionChanged += AudioPlayer_CurrentTimeChanged;
            playbackSession.PlaybackStateChanged += Player_PlaybackStateChanged;
        }

        private void OpenCoverFile()
        {
            if (Cover == null) return;
            string ImageFilePath = AppContext.BaseDirectory + @"\temp_cover.png";
            Helper.File.SaveBitmapImageToPng(Cover, ImageFilePath);
            Helper.File.OpenFileWithDefaultApp(ImageFilePath);
        }


        private void MuteUnmute()
        {
            IsMuted = !IsMuted;
        }

        private void UpdateTotalTime() => NotifyPropertyChanged(nameof(TotalTime));
        private void UpdateCurrentTime() => NotifyPropertyChanged(nameof(CurrentTime));
        private async void Player_PlaybackStateChanged(PlaybackState playbackState)
        {
            NotifyPropertyChanged(nameof(CurrentPlaybackState));
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
                    //Cover =  await CoverImage.AlbumArtAsync(Player.Controller.AudioFilePath);
                    UpdateAll();
                    playbackSession.Play();
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
                    await playbackSession.OpenAsync(playbackSession.AudioFilePath);
                    break;
                case PlaybackState.Closed:
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

        private async void PlayPause()
        {
            switch (player.PlaybackSession.PlaybackState)
            {
                case PlaybackState.Playing:
                    player.PlaybackSession.Pause();
                    break;
                case PlaybackState.Ended:
                    await timelineController.SeekAsync(TimeSpan.Zero);
                    player.PlaybackSession.Play();
                    break;
                case PlaybackState.Closed:
                    Commands.FilePicker.OpenFilePicker(player);
                    break;
                default:
                    player.PlaybackSession.Play();
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



        private TimeSpan _currentTime;
        public TimeSpan CurrentTime
        {
            get => timelineController.Current;
            set => Task.Run(async () => await timelineController.SeekAsync(value));
        }

        private double _volume = App.Player.PlaybackSession.VolumeController.Volume;
        public double Volume
        {
            get => _volume;
            set => volumeController.ChangeVolume((int)value);
        }

        public bool IsPlaying => playbackSession.IsPlaying;

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
                    if (!string.IsNullOrEmpty(playbackSession.AudioFilePath))
                    {
                        if (playbackSession.AudioInfo == null) return "empty";
                        AudioInfo audioInfo = playbackSession.AudioInfo;
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



    }
}