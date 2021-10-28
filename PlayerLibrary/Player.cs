using PlayerLibrary.Core;
using PlayerLibrary.Model;
using System;
using System.Threading.Tasks;
using System.Windows;
using static PlayerLibrary.Events;

namespace PlayerLibrary
{
    public class Player
    {
        #region base
        private readonly NAudioCore nAudioCore;

        public Player()
        {
            nAudioCore = new(this);
        }
        #endregion

        #region Void
        public void Play() => nAudioCore.Play();
        public void Pause() => nAudioCore.Pause();
        public void Close() => nAudioCore.Close();
        public void Stop() => nAudioCore.Stop();
        public void Seek(double value) => nAudioCore.Seek(TimeSpan.FromSeconds(value));

        #endregion

        #region Async
        public async Task OpenAsync(string filepath) => await nAudioCore.OpenAsync(filepath);
        public async Task SeekAsync(TimeSpan value) => await nAudioCore.SeekAsync(value);

        #endregion

        #region get and set
        public string Source
        {
            get => nAudioCore.Source;
        }
        public TimeSpan TimePosition { get => nAudioCore.CurrentTime; set => nAudioCore.CurrentTime = value; }
        public EqualizerMode EqualizerMode { get => nAudioCore.equalizerMode; set => nAudioCore.equalizerMode = value; }
        #endregion

        #region get
        public TimeSpan TimeDuration => nAudioCore.TotalTime;
        public PlaybackState PlaybackState => nAudioCore.PlaybackState;
        public ReaderInfo ReaderInfo => nAudioCore.ReaderInfo;
        #endregion

        #region Events
        public event EventHandlerPlaybackState PlaybackStateChanged;

        internal void InvokePlaybackStateChanged(PlaybackState value)
        {
            if (IsEventsOn)
            {
                PlaybackStateChanged?.Invoke(value);
            }
        }

        public event EventHandlerTimeSpan TimePositionChanged;
        internal async void InvokeCurrentTime(TimeSpan timespan)
        {
            if (IsEventsOn)
            {
                await Task.Run(() => TimePositionChanged?.Invoke(timespan));
            }
        }

        #endregion

        #region Volume
        public event EventHandlerVolume VolumeChanged;
        internal void InvokeVolumeChanged(int newVolume)
        {
            if (IsEventsOn)
            {
                VolumeChanged?.Invoke(newVolume);
            }
        }

        public int Volume { get => nAudioCore.Volume; private set => nAudioCore.Volume = value; }
        public bool IsMuted { get => nAudioCore.IsMuted; set => nAudioCore.IsMuted = value; }

        public void VolumeUp(int value) => ChangeVolume(nAudioCore.Volume += value);
        public void VolumeDown(int value) => ChangeVolume(nAudioCore.Volume -= value);
        public void ChangeVolume(int newVolume)
        {
            try
            {
                if (newVolume is >= 0 and <= 100)
                {
                    nAudioCore.Volume = newVolume;
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region Eq
        public EventHandlerEmpty EqUpdated;
        private void FireEqUpdated()
        {   
            if (IsEventsOn)
            {
                EqUpdated?.Invoke();
            }
        }
        public void ResetEq() => nAudioCore.ResetEq();
        public void ChangeEq(int bandIndex, float Gain,bool requestNotifyEqUpdate) 
        {
            nAudioCore.ChangeEqualizerBand(bandIndex, Gain);
           if (requestNotifyEqUpdate)
           {
               FireEqUpdated();
           } 
        }
        public double GetEqBandGain(int BandIndex) => nAudioCore.GetEqBandGain(BandIndex);
        public int[] EqBandsGain { get => nAudioCore.BandsGain; }
        #endregion   

        private bool IsEventsOn = true;
        private void ToggleEventsOff() => IsEventsOn = false;
        private void ToggleEventsOn() => IsEventsOn = true;

        public void ReIntialEq()
        {
            PlaybackState lastpstate = PlaybackState;
            ToggleEventsOff();

            nAudioCore.InitalEqualizer();
            if (!string.IsNullOrEmpty(nAudioCore.Source))
            {
                nAudioCore.Open(Source, TimePosition);    
                switch (lastpstate)
                {
                    case PlaybackState.Paused:
                        Pause();
                        break;
                    case PlaybackState.Playing:
                        Play();
                        break;
                    case PlaybackState.Stopped:
                        Stop();
                        break;
                    default:
                        break;
                }
            }
            
            ToggleEventsOn();
            FireEqUpdated();
        }

    }
}
