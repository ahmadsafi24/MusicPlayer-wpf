using Engine.Enums;
using Engine.Internal;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Engine
{
    public class Player
    {
        #region base
        private readonly NAudioCore core;
        public readonly PlaylistManager PlaylistManager;

        public Player()
        {
            core = new(this);
            PlaylistManager = new(this);
        }
        #endregion


        public void Play() => core.Play();
        public void Pause() => core.Pause();
        public void Close() => core.Close();
        public void Stop() => core.Stop();
        public void Seek(double value) => core.Seek(value);

        #region Async
        public async Task OpenAsync() => await core.OpenAsync();
        public async Task SeekAsync(double value) => await core.SeekAsync(value);

        #endregion

        #region get and set
        public string Source { get => core.Source; set => core.Source = value; }
        public TimeSpan Position { get => core.CurrentTime; set => core.CurrentTime = value; }

        #endregion

        #region get
        public TimeSpan CurrentTime => core.CurrentTime;
        public TimeSpan TotalTime => core.TotalTime;
        public PlaybackState PlaybackState => core.PlaybackState;

        #endregion

        #region Events
        public event EventHandlerPlaybackState PlaybackStateChanged;
        internal void InvokePlaybackStateChanged(PlaybackState value) => PlaybackStateChanged?.Invoke(value);

        public event EventHandlerTimeSpan CurrentTimeChanged;
        internal async void InvokeCurrentTime(TimeSpan timespan) => await Task.Run(() => CurrentTimeChanged?.Invoke(timespan));

        #endregion

        #region Volume
        public event EventHandlerVolume VolumeChanged;
        internal void InvokeVolumeChanged(int newVolume) => VolumeChanged?.Invoke(newVolume);

        public int Volume { get => core.Volume; private set => core.Volume = value; }
        public void VolumeUp(int value) => ChangeVolume(core.Volume += value);
        public void VolumeDown(int value) => ChangeVolume(core.Volume -= value);
        public void ChangeVolume(int newVolume)
        {
            try
            {
                if (newVolume is >= 0 and <= 100)
                {
                    core.Volume = newVolume;
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region Eq
        public void ResetEq() => core.ResetEq();
        public void ChangeEq(int bandIndex, float Gain) => core.ChangeEqualizerBand(bandIndex, Gain);
        public double GetEqBandGain(int BandIndex) => core.GetEqBandGain(BandIndex);

        #endregion
    }
}
