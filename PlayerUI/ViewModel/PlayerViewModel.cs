namespace PlayerUI.ViewModel
{
    public class PlayerViewModel : ViewModelBase
    {
        public Player Player => App.Player;
        private PlaybackSession PlaybackSession => App.Player.PlaybackSession;
        private TimelineController TimelineController => App.Player.PlaybackSession.TimelineController;
        private VolumeController VolumeController => App.Player.PlaybackSession.VolumeController;

        #region Commands
        public ICommand PlayPauseCommand { get; }
        public ICommand MuteAudioCommand { get; }
        public ICommand OpenCoverFileCommand { get; }
        public ICommand NextAudioCommand { get; }
        public ICommand PreviousAudioCommand { get; }

        #endregion

        public PlayerViewModel()
        {
            PlayPauseCommand = new DelegateCommand(PlayPause);
            MuteAudioCommand = new DelegateCommand(MuteUnmute);
            OpenCoverFileCommand = new DelegateCommand(OpenCoverFile);

            NextAudioCommand = new DelegateCommand(NextAudio);
            PreviousAudioCommand = new DelegateCommand(PreviousAudio);

            TimelineController.TimePositionChanged += AudioPlayer_CurrentTimeChanged;
            PlaybackSession.PlaybackStateChanged += Player_PlaybackStateChanged;
            Player.PlaybackSession.NAudioPlayerChanged += PlaybackSession_NAudioPlayerChanged;
            VolumeController.VolumeChanged += AudioPlayer_VolumeChanged;
        }

        private void PlaybackSession_NAudioPlayerChanged(Type type)
        {

        }

        private void OpenCoverFile()
        {
            if (Cover == null)
            {
                return;
            }

            string ImageFilePath = AppContext.BaseDirectory + @"\temp_cover.png";
            FileHelper.SaveBitmapImageToPng(Cover, ImageFilePath);
            FileHelper.OpenFileWithDefaultApp(ImageFilePath);
        }


        private void MuteUnmute()
        {
            IsMuted = !IsMuted;
        }

        private void Player_PlaybackStateChanged(PlaybackState playbackState)
        {


            switch (playbackState)
            {
                case PlaybackState.Unknown:
                    break;
                case PlaybackState.Failed:
                    break;
                case PlaybackState.Opened:
                    NotifyPropertyChanged(nameof(TagFile));
                    NotifyPropertyChanged(nameof(CoreCurrentFileInfo));
                    PlaybackSession.Play();
                    return;
                case PlaybackState.Paused:
                    Taskbar.SetTaskbarState(ProgressState.Paused);
                    NotifyPropertyChanged(nameof(CurrentPlaybackState));
                    NotifyPropertyChanged(nameof(IsPlaying));
                    return;
                case PlaybackState.Playing:
                    break;
                case PlaybackState.Stopped:
                    break;
                case PlaybackState.Ended:
                    break;
                case PlaybackState.Closed:
                    break;
                case PlaybackState.None:
                    break;
                default:
                    break;
            }
            Taskbar.SetTaskbarState(ProgressState.Normal);
            NotifyPropertyChanged(nameof(CurrentPlaybackState));
            NotifyPropertyChanged(nameof(IsPlaying));
            NotifyPropertyChanged(nameof(TotalTime));
            NotifyPropertyChanged(nameof(CurrentTime));
        }

        private void PlayPause()
        {
            switch (Player.PlaybackSession.PlaybackState)
            {
                case PlaybackState.Playing:
                    Player.PlaybackSession.Pause();
                    break;
                case PlaybackState.Ended:
                    TimelineController.Seek(TimeSpan.Zero);
                    Player.PlaybackSession.Play();
                    break;
                case PlaybackState.Closed:
                    FilePicker.OpenFilePicker(Player);
                    break;
                default:
                    Player.PlaybackSession.Play();
                    break;
            }
        }

        private void AudioPlayer_VolumeChanged(float newVolume)
        {
            NotifyPropertyChanged(nameof(Volume));
            NotifyPropertyChanged(nameof(IsMuted));
        }

        private void AudioPlayer_CurrentTimeChanged(TimeSpan Time)
        {
            NotifyPropertyChanged(nameof(CurrentTime));
            Taskbar.SetTaskbarProgressValue(Time, TotalTime);
        }


        public TimeSpan CurrentTime
        {
            get => TimelineController.Current;
            set => Task.Run(async () => await TimelineController.SeekAsync(value));
        }

        public bool IsPlaying => PlaybackSession.IsPlaying;
        public float Volume
        {
            get => VolumeController.Volume;
            set => VolumeController.ChangeVolume(value);
        }

        public bool IsMuted
        {
            get => VolumeController.IsMuted;
            set => VolumeController.IsMuted = value;
        }
        public string CurrentPlaybackState => PlaybackSession.PlaybackState.ToString();
        private BitmapImage Cover => TagFile.AlbumArt;
        public AudioTag TagFile => PlaybackSession.AudioInfo?.AudioTag;
        public TimeSpan TotalTime => TimelineController.Total;

        private void NextAudio()
        {

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
                    if (!string.IsNullOrEmpty(PlaybackSession.CurrentTrackFile))
                    {
                        if (PlaybackSession.AudioInfo == null)
                        {
                            return "empty";
                        }

                        AudioInfo audioInfo = PlaybackSession.AudioInfo;
                        return $"{audioInfo.Format} {audioInfo.BitrateString} {audioInfo.SampleRate / 1000}kHz {audioInfo.Channels}ch";

                        // not working return $"{TagFile.AudioFormatName} {TagFile.Bitrate} {TagFile.SampleRate} {TagFile.Channels}";

                    }
                    else
                    {
                        return "";
                    }
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }
    }
}