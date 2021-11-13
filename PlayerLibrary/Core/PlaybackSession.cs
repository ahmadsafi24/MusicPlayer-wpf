using Helper;
using NAudio.Wave;
using PlayerLibrary.Core;
using PlayerLibrary.Core.NAudioPlayer.Interface;
using System;
using System.Threading.Tasks;
using static PlayerLibrary.Events;

namespace PlayerLibrary.Core
{
    public class PlaybackSession
    {
        public string CurrentTrackFile {get; set;}

        public bool RepeatCurrentTrack = false;
        public event EventHandlerType NAudioPlayerChanged;
        private INAudioPlayer _nAudioPlayer;
        internal INAudioPlayer NAudioPlayer
        {
            get => _nAudioPlayer;
            set
            {
                Log.WriteLine("playbacksession.naudioplayer changed to type: " + value.GetType());

                if (_nAudioPlayer != null)
                {
                    _nAudioPlayer.Dispose();
                }

                _nAudioPlayer = value;
                Intialize();
                InvokeNAudioPlayerChanged(value.GetType());
            }
        }
        public Type NAudioPlayerType => NAudioPlayer.GetType();
        public bool IsEqualizerEnabled => NAudioPlayer.GetType() == typeof(NAudioPlayer.NAudioPlayerEq);

        internal void InvokeNAudioPlayerChanged(Type type)
        {
            NAudioPlayerChanged?.Invoke(type);
        }

        public TimelineController TimelineController;

        public VolumeController VolumeController;

        internal PlaybackSession(INAudioPlayer nAudioPlayer)
        {
            Log.WriteLine("new PlaybackSession with type: " + nAudioPlayer.GetType());
            this.NAudioPlayer = nAudioPlayer;
            TimelineController = new(this);
            VolumeController = new(this);
        }

        private void Intialize()
        {
            if (TimelineController != null)
            {
                TimelineController.NAudioPlayer = NAudioPlayer;
            }

            if (VolumeController != null)
            {
                VolumeController.NAudioPlayer = NAudioPlayer;
            }

            NAudioPlayer.OutputDevice.PlaybackStopped += WaveOutEvent_PlaybackStopped;
            Log.WriteLine("PlaybackSession Initialized");
        }
        public async Task OpenAsync(string filePath) => await Task.Run(() => { CoreOpen(filePath); });

        public void Open(string filePath, TimeSpan timePosition)
        {
            Log.WriteLine("open with time", filePath);
            CoreOpen(filePath);
            NAudioPlayer.Reader.CurrentTime = timePosition;
        }

        public void Open(string filePath) => CoreOpen(filePath);
        public void Play()
        {
            Log.WriteLine("Play");
            try
            {
                if (PlaybackState != PlaybackState.Playing && IsFileOpen == true)
                {
                    NAudioPlayer.OutputDevice.Play();
                    TriggerStatePlaying();
                }
                else if (IsFileOpen == false)
                {
                    if (!string.IsNullOrEmpty(CurrentTrackFile))
                    {
                        CoreOpen(CurrentTrackFile);
                        NAudioPlayer.OutputDevice.Play();
                        TriggerStatePlaying();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine("play", ex.Message);
            }
        }

        public void Pause()
        {
            Log.WriteLine("Pause");
            NAudioPlayer.OutputDevice.Pause();
            TriggerStatePaused();
        }
        public void Stop()
        {
            Log.WriteLine("Stop");
            NAudioPlayer.OutputDevice.Stop();
            NAudioPlayer.Reader.CurrentTime = TimeSpan.Zero;
            TriggerStateStopped();
        }

        public void Close()
        {
            Stop();
            NAudioPlayer.OutputDevice.Dispose();
            NAudioPlayer.Reader.Close();
            CurrentTrackFile = null;
            NAudioPlayer.Reader.Dispose();
            NAudioPlayer.Reader = null;
            Log.WriteLine("Close: done");
            TriggerStateClosed();
        }
        private PlaybackState _playbackState = PlaybackState.None;
        public PlaybackState PlaybackState
        {
            get => _playbackState;
            private set
            {
                _playbackState = value;
                InvokePlaybackStateChanged(value);
            }
        }

        private void WaveOutEvent_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Exception?.Message))
            {
                if ((int)NAudioPlayer.Reader.CurrentTime.TotalSeconds >= (int)NAudioPlayer.Reader.TotalTime.TotalSeconds - 1)
                {
                    TriggerStateEnded();
                }
            }
            else if (string.IsNullOrEmpty(e.Exception?.Message))
            {
                Log.WriteLine("WaveOutEvent_PlaybackStopped With No Exception");
                TriggerStateStopped();
            }
            else
            {
                Log.WriteLine(e.Exception.Message + "WaveOutEvent_PlaybackStopped");
                TriggerStateFailed();
            }
        }

        #region Events
        public event EventHandlerPlaybackState PlaybackStateChanged;

        internal void InvokePlaybackStateChanged(PlaybackState value)
        {
            if (IsEventsOn)
            {
                Log.WriteLine("PlaybackState: " + value.ToString());
                PlaybackStateChanged?.Invoke(value);
            }
        }

        #endregion

        private bool IsEventsOn { get; set; } = true;
        internal void ToggleEventsOff() => IsEventsOn = false;
        internal void ToggleEventsOn() => IsEventsOn = true;


        public FileInfo.AudioInfo AudioInfo => new(CurrentTrackFile);


        public bool IsFileOpen = false;

        public bool IsPlaying = false;

        private bool CheckFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Log.WriteLine("CheckFile: filePath is null");
                return false;
            }
            else if (!System.IO.File.Exists(filePath))
            {
                Log.WriteLine("CheckFile: filePath not exists");
                return false;
            }
            else
            {
                return true;
            }
        }

        private void CloseIfOpen()
        {
            Log.WriteLine("CloseIfOpen", $"current pstate { PlaybackState}");
            if (PlaybackState is PlaybackState.Playing
                              or PlaybackState.Paused
                              or PlaybackState.Failed)
            {
                Close();
            }
        }

        private void CoreOpen(string filePath)
        {
            Log.WriteLine("CoreOpen", filePath);
            try
            {
                CloseIfOpen();
                if (CheckFile(filePath) == false) return;
                CurrentTrackFile = filePath;

                NAudioPlayer.Reader = new MediaFoundationReader(filePath);
                NAudioPlayer.Init();

                if (NAudioPlayer.Reader.TotalTime.TotalSeconds <= 0)
                {
                    TriggerStateFailed();
                    return;
                }
                else
                {
                    TriggerStateOpened();
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine("CoreOpen", ex.Message);
            }
        }

        #region FirePaybackState

        private void TriggerStateFailed()
        {
            IsFileOpen = false;
            IsPlaying = false;
            PlaybackState = PlaybackState.Failed;
        }

        private void TriggerStateOpened()
        {
            IsFileOpen = true;
            IsPlaying = false;
            PlaybackState = PlaybackState.Opened;
        }

        private void TriggerStatePlaying()
        {
            IsPlaying = true;
            PlaybackState = PlaybackState.Playing;
        }

        private void TriggerStatePaused()
        {
            IsPlaying = false;
            PlaybackState = PlaybackState.Paused;
        }

        private void TriggerStateStopped()
        {
            IsPlaying = false;
            PlaybackState = PlaybackState.Stopped;
        }

        private void TriggerStateClosed()
        {
            IsPlaying = false;
            IsFileOpen = false;
            PlaybackState = PlaybackState.Closed;
        }

        private void TriggerStateEnded()
        {
            IsPlaying = false;
            PlaybackState = PlaybackState.Ended;
            if (RepeatCurrentTrack == true)
            {
                CoreOpen(CurrentTrackFile);
            }
        }
        #endregion

    }
}