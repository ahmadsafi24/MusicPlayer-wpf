using Helper;
using NAudio.Wave;
using PlayerLibrary.Core;
using System;
using System.Threading.Tasks;
using static PlayerLibrary.Events;

namespace PlayerLibrary.Shell
{
    public class PlaybackSession
    {
        public string AudioFilePath { get; set; }
        internal readonly NAudioCore nAudioCore;

        public readonly TimelineController TimelineController;
        public readonly VolumeController VolumeController;

        internal PlaybackSession(NAudioCore nAudioCore)
        {
            this.nAudioCore = nAudioCore;
            TimelineController = new(this);
            VolumeController = new(this);
            Intialize();
        }

        private void Intialize()
        {
            nAudioCore.WaveOutEvent.PlaybackStopped += WaveOutEvent_PlaybackStopped;
            Log.WriteLine("PlayerInitialized");
        }
        public async Task OpenAsync(string filePath)
        {
            await Task.Run(() =>
            {
                CoreOpen(filePath);
            });
        }
        public void Open(string filePath, TimeSpan timePosition)
        {
            CoreOpen(filePath);
            nAudioCore.Reader.CurrentTime = timePosition;
        }

        public void Open(string filePath) => CoreOpen(filePath);
        public void Play()
        {
            try
            {
                if (PlaybackState != PlaybackState.Playing && IsFileOpen == true)
                {
                    nAudioCore.WaveOutEvent.Play();
                    TriggerStatePlaying();
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex);
                throw;
            }
        }

        public void Pause()
        {
            nAudioCore.WaveOutEvent.Pause();
            TriggerStatePaused();
        }
        public void Stop()
        {
            nAudioCore.WaveOutEvent.Stop();
            nAudioCore.Reader.CurrentTime = TimeSpan.Zero;
            TriggerStateStopped();
        }

        public void Close()
        {
            nAudioCore.WaveOutEvent.Dispose();
            nAudioCore.Reader.Close();
            AudioFilePath = null;
            nAudioCore.Reader.Dispose();
            nAudioCore.Reader = null;
            TriggerStateClosed();
        }
        private PlaybackState _playbackState = PlaybackState.None;
        public PlaybackState PlaybackState
        {
            get => _playbackState;
            private set
            {
                //Info = new(AudioFilePath);
                Log.WriteLine("PlaybackState: " + value.ToString());
                _playbackState = value;
                if (IsEventsOn)
                {
                    InvokePlaybackStateChanged(value);
                }
            }
        }

        private void WaveOutEvent_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Exception?.Message))
            {
                if ((int)nAudioCore.Reader.CurrentTime.TotalSeconds >= (int)nAudioCore.Reader.TotalTime.TotalSeconds - 1)
                {
                    PlaybackState = PlaybackState.Ended;
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
            PlaybackStateChanged?.Invoke(value);
        }

        #endregion

        private bool IsEventsOn = true;
        internal void ToggleEventsOff() => IsEventsOn = false;
        internal void ToggleEventsOn() => IsEventsOn = true;


        public FileInfo.AudioInfo AudioInfo;

        internal bool IsEqEnabled = false;

        public bool IsFileOpen = false;

        public bool IsPlaying = false;

        private bool CheckFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Log.WriteLine("Core: parameter path is null");
                return false;
            }
            else if (!System.IO.File.Exists(filePath))
            {
                Log.WriteLine("Core: parameter path is null");
                return false;
            }
            else
            {
                return true;
            }
        }

        private void CloseIfOpen()
        {
            if (PlaybackState is PlaybackState.Playing or PlaybackState.Paused)
            {
                Stop();
                Close();
            }
        }

        private void CoreOpen(string filePath)
        {
            try
            {
                CloseIfOpen();
                if (CheckFile(filePath) == false) return;
                AudioFilePath = filePath;

                nAudioCore.Reader = new(filePath);

                if (IsEqEnabled == true)
                {
                    CoreWithEq();
                }
                else
                {
                    CoreNormal();
                }

                AudioInfo = new(filePath);

                if (nAudioCore.Reader.TotalTime.TotalSeconds <= 0)
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
                Log.WriteLine(ex.Message);
                throw;
            }
        }

        private void CoreNormal()
        {
            nAudioCore.WaveOutEvent.Init(nAudioCore.Reader);
        }

        private void CoreWithEq()
        {
            nAudioCore.EqualizerCore = new(nAudioCore.SampleProvider, nAudioCore.EqualizerBand);
            nAudioCore.WaveOutEvent.Init(nAudioCore.EqualizerCore);
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

        #endregion

    }
}