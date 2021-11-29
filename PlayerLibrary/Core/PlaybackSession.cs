using Helper;

using NAudio.Wave;
using PlayerLibrary.Bridge;
using System;
using System.IO;
using System.Threading.Tasks;
using static PlayerLibrary.Events;

namespace PlayerLibrary.Core
{
    public class PlaybackSession
    {
        public NAudioPlayer audioPlayer = new();
        public Uri CurrentTrackFile { get; private set; }
        public bool AutoPlay { get; set; } = true;
        public bool RepeatCurrentTrack = false;

        public TimelineController TimelineController;

        public VolumeController VolumeController = new();

        public EffectContainer EffectContainer;
        internal PlaybackSession()
        {
            TimelineController = new(this);
            EffectContainer = new();
            Log.WriteLine("PlaybackSession Initialized");
        }

        public async Task OpenAsync(Uri filePath)
        {
            await Task.Run(() => { CoreOpen(filePath); });
        }

        public void Open(Uri filePath, TimeSpan timePosition)
        {
            Log.WriteLine("open with time", filePath);
            PlaybackState tempPlaybackState = PlaybackState;
            CoreOpen(filePath);
            audioPlayer.Reader.CurrentTime = timePosition;
            if (tempPlaybackState== PlaybackState.Playing)
            {
                Play();
            }
        }

        public void Open(Uri filePath)
        {
            CoreOpen(filePath);
        }

        public void Play()
        {
            Log.WriteLine("Play");
            try
            {
                if (PlaybackState != PlaybackState.Playing && IsFileOpen == true)
                {
                    audioPlayer.OutputDevice.Play();
                    RaisePlaybackStatePlaying();
                }
                else if (IsFileOpen == false)
                {
                    if (CurrentTrackFile != null)
                    {
                        CoreOpen(CurrentTrackFile);
                        audioPlayer.OutputDevice.Play();
                        RaisePlaybackStatePlaying();
                    }
                }
            }
            catch (Exception ex)
            {
                RaiseOnMessageLogged($"On Play():\n{ex.Message}");
            }
        }

        public void Pause()
        {
            Log.WriteLine("Pause");
            audioPlayer.OutputDevice.Pause();
            RaisePlaybackStatePaused();
        }
        public void Stop()
        {
            Log.WriteLine("Stop");
            audioPlayer.OutputDevice.Stop();
            audioPlayer.Reader.CurrentTime = TimeSpan.Zero;
            RaisePlaybackStateStopped();
        }

        public void Close()
        {
            Stop();
            try
            {
                audioPlayer.OutputDevice.Dispose();
                audioPlayer.Reader.Dispose();
                CurrentTrackFile = null;
                Log.WriteLine("Close: done");

            }
            catch (System.Exception ex)
            {
                RaiseOnMessageLogged($"On Close(): \n{ex.Message}");
            }
            RaisePlaybackStateClosed();
        }
        private PlaybackState _playbackState = PlaybackState.None;
        public PlaybackState PlaybackState
        {
            get => _playbackState;
            private set
            {
                _playbackState = value;
                RaisePlaybackStateChanged(value);
            }
        }

        private void WaveOutEvent_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Exception?.Message))
            {
                if ((int)audioPlayer.Reader.CurrentTime.TotalSeconds >= (int)audioPlayer.Reader.TotalTime.TotalSeconds - 1)
                {
                    RaisePlaybackStateEnded();
                }
            }
            else if (string.IsNullOrEmpty(e.Exception?.Message))
            {
                Log.WriteLine("WaveOutEvent_PlaybackStopped With No Exception");
                RaisePlaybackStateStopped();
            }
            else
            {
                RaiseOnMessageLogged("WaveOutEvent_PlaybackStopped: \n" + e.Exception.Message);
                RaisePlaybackStateFailed();
            }
        }

        #region Events
        public event EventHandlerPlaybackState PlaybackStateChanged;

        internal void RaisePlaybackStateChanged(PlaybackState value)
        {
            if (IsEventsOn)
            {
                Log.WriteLine("PlaybackState: " + value.ToString());
                PlaybackStateChanged?.Invoke(value);
            }
        }

        #endregion

        private bool IsEventsOn { get; set; } = true;
        internal void ToggleEventsOff()
        {
            IsEventsOn = false;
        }

        internal void ToggleEventsOn()
        {
            IsEventsOn = true;
        }

        public FileInfo.AudioInfo AudioInfo => new(CurrentTrackFile);


        public bool IsFileOpen = false;

        public bool IsPlaying = false;

        private bool CheckFile(Uri filePath)
        {
            if (filePath == null)
            {
                RaiseOnMessageLogged("CheckFile: filePath is null");
                return false;
            }
            else if (filePath.IsFile)
            {
                if (!File.Exists(filePath.OriginalString))
                {
                    RaiseOnMessageLogged("CheckFile: filePath not exists");
                    return false;
                }
                else
                {
                    return true;
                }
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
        private void CoreOpen(Uri filePath)
        {
            Log.WriteLine("CoreOpen", filePath);
            try
            {
                CloseIfOpen();
                if (CheckFile(filePath) == false)
                {
                    return;
                }

                CurrentTrackFile = filePath;

                audioPlayer.Reader = new StreamMediaFoundationReader(new FileStream(filePath.OriginalString, FileMode.Open, FileAccess.Read));

                VolumeController.Source = audioPlayer.Reader.ToSampleProvider();
                

                EffectContainer.Source = VolumeController;
                EffectContainer.InitEffects();

                audioPlayer.SampleProvider = EffectContainer;
            
                audioPlayer.OutputDevice = new WaveOutEvent();

                audioPlayer.OutputDevice.PlaybackStopped += WaveOutEvent_PlaybackStopped;
                audioPlayer.Init();
                if (audioPlayer.Reader.TotalTime.TotalSeconds <= 0)
                {
                    RaisePlaybackStateFailed();
                    return;
                }
                else
                {
                    RaisePlaybackStateOpened();
                    if (AutoPlay==true)
                    {
                        Play();
                    }
                }
            }
            catch (Exception ex)
            {
                RaiseOnMessageLogged("On CoreOpen(uri filePath): \n"+ex.Message);
                //throw;
            }
        }

        #region FirePaybackState

        private void RaisePlaybackStateFailed()
        {
            IsFileOpen = false;
            IsPlaying = false;
            PlaybackState = PlaybackState.Failed;
        }

        private void RaisePlaybackStateOpened()
        {
            IsFileOpen = true;
            IsPlaying = false;
            PlaybackState = PlaybackState.Opened;
        }

        private void RaisePlaybackStatePlaying()
        {
            IsPlaying = true;
            PlaybackState = PlaybackState.Playing;
        }

        private void RaisePlaybackStatePaused()
        {
            IsPlaying = false;
            PlaybackState = PlaybackState.Paused;
        }

        private void RaisePlaybackStateStopped()
        {
            IsPlaying = false;
            PlaybackState = PlaybackState.Stopped;
        }

        private void RaisePlaybackStateClosed()
        {
            IsPlaying = false;
            IsFileOpen = false;
            PlaybackState = PlaybackState.Closed;
        }

        private void RaisePlaybackStateEnded()
        {
            IsPlaying = false;
            PlaybackState = PlaybackState.Ended;
            if (RepeatCurrentTrack == true)
            {
                CoreOpen(CurrentTrackFile);
            }
        }
        #endregion

        public event EventHandlerMessageLog OnMessageLogged;
        private void RaiseOnMessageLogged(string message)
        {
            OnMessageLogged?.Invoke(message);
        }

    }
}