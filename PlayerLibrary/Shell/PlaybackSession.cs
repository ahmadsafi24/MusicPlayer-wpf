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
        internal PlaybackSession(NAudioCore nAudioCore)
        {
            this.nAudioCore = nAudioCore;
            TimelineController = new(this);
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
                if (PlaybackState != PlaybackState.Playing)
                {
                    nAudioCore.WaveOutEvent.Play();
                    PlaybackState = PlaybackState.Playing;
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex);
            }

        }

        public void Pause()
        {
            nAudioCore.WaveOutEvent.Pause();
            PlaybackState = PlaybackState.Paused;
        }
        public void Stop()
        {
            nAudioCore.WaveOutEvent.Stop();
            nAudioCore.Reader.CurrentTime = TimeSpan.Zero;
            PlaybackState = PlaybackState.Stopped;
        }

        public void Close()
        {
            nAudioCore.WaveOutEvent.Dispose();
            nAudioCore.Reader.Close();
            AudioFilePath = null;
            nAudioCore.Reader.Dispose();
            nAudioCore.Reader = null;
            PlaybackState = PlaybackState.Closed;
        }
        private PlaybackState _playbackState;
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
                PlaybackState = PlaybackState.Stopped;
            }
            else
            {
                Log.WriteLine(e.Exception.Message + "WaveOutEvent_PlaybackStopped");
                PlaybackState = PlaybackState.Failed;
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
                if (CheckFile(filePath) == false) return;
                AudioFilePath = filePath;

                CloseIfOpen();
                nAudioCore.Reader = new(filePath);


                if (IsEqEnabled == false)
                {
                    CoreNormal();
                }
                else
                {
                    CoreWithEq();
                }

                AudioInfo = new(filePath);

                if (nAudioCore.Reader.TotalTime.TotalSeconds <= 0)
                {
                    PlaybackState = PlaybackState.Failed;
                    return;
                }
                else
                {
                    PlaybackState = PlaybackState.Opened;
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message);
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
    }
}