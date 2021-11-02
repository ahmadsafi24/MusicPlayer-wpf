using NAudio.Wave;
using PlayerLibrary.Core;
using System;
using System.Threading.Tasks;

namespace PlayerLibrary.Shell
{
    public class Controller
    {
        public string AudioFilePath { get; set; }
        private NAudioCore nAudioCore;

        private Player player;

        internal Controller(NAudioCore nAudioCore)
        {
            this.nAudioCore = nAudioCore;
            Intialize();
        }

        internal Controller(Player player)
        {
            this.player = player;
            this.nAudioCore = player.nAudioCore;
            Intialize();
        }
        private void Intialize()
        {
            nAudioCore.WaveOutEvent.PlaybackStopped += WaveOutEvent_PlaybackStopped;
            Log.WriteLine("PlayerInitialized");
        }
        public void Open(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    Log.WriteLine("Core: parameter path is null");
                    return;
                }
                if (PlaybackState is PlaybackState.Playing or PlaybackState.Paused)
                {
                    Stop();
                    Close();
                }
                AudioFilePath = filePath;
                nAudioCore.Reader = new(filePath);
                if (nAudioCore.Reader.TotalTime.TotalSeconds <= 0) { PlaybackState = PlaybackState.Failed; return; }
                if (nAudioCore.equalizerMode == EqualizerMode.Disabled)
                {
                    nAudioCore.WaveOutEvent.Init(nAudioCore.Reader);
                }
                else
                {
                    nAudioCore.EqualizerCore = new(nAudioCore.SampleProvider, nAudioCore.EqualizerBand);
                    nAudioCore.WaveOutEvent.Init(nAudioCore.EqualizerCore);
                }
                AudioInfo = new(filePath);
                PlaybackState = PlaybackState.Opened;
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message);
            }
        }
        public async Task OpenAsync(string filePath)
        {
            await Task.Run(() =>
            {
                Open(filePath);
            });

        }
        public void Open(string filePath, TimeSpan timePosition)
        {
            Open(filePath);
            nAudioCore.Reader.CurrentTime = timePosition;
        }

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
                    player.InvokePlaybackStateChanged(value);
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

        private bool IsEventsOn = true;
        private void ToggleEventsOff() => IsEventsOn = false;
        private void ToggleEventsOn() => IsEventsOn = true;


        public FileInfo.AudioInfo AudioInfo;

    }
}