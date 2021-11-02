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

        internal Player(NAudioCore nAudioCore)
        {
            this.nAudioCore = nAudioCore;
            nAudioCore.CurrentTimeWatcherInterval = 0.5;//half second
        }
        public Player()
        {
            this.nAudioCore = new NAudioCore(this);
            nAudioCore.CurrentTimeWatcherInterval = 0.5;//half second
        }
        #endregion

        #region Void
        public void Open(string filePath) => nAudioCore.Open(filePath);
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

            PlaybackStateChanged?.Invoke(value);

        }

        public event EventHandlerTimeSpan TimePositionChanged;
        internal async void InvokeCurrentTime(TimeSpan timespan)
        {
            await Task.Run(() => TimePositionChanged?.Invoke(timespan));

        }

        #endregion

        #region Volume
        public event EventHandlerVolume VolumeChanged;
        internal void InvokeVolumeChanged(int newVolume)
        {
            VolumeChanged?.Invoke(newVolume);

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

        public void ResetEq() => nAudioCore.ResetEq();

        public void ChangeEq(int bandIndex, float Gain, bool requestNotifyEqUpdate) => nAudioCore.ChangeEq(bandIndex, Gain, requestNotifyEqUpdate);

        public double GetEqBandGain(int BandIndex) => nAudioCore.GetEqBandGain(BandIndex);

        public int[] EqBandsGain { get => nAudioCore.BandsGain; }

        public void ReIntialEq() => nAudioCore.ReIntialEq();

        public void ChangeBands(int[] bands) => nAudioCore.ChangeBands(bands);

        public void ImportEq(EqPreset preset) => ChangeBands(preset.BandsGain);

        public void ExportEq(string filepath)
        {
            EqPreset EqPreset = new(EqualizerMode, EqBandsGain);
            Preset.Equalizer.PresetToFile(EqPreset, filepath);
        }

        public EventHandlerEmpty EqUpdated;
        internal void FireEqUpdated()
        {
            EqUpdated?.Invoke();
        }
        #endregion
    }
}
