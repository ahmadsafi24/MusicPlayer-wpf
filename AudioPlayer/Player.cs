using AudioPlayer.Core;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace AudioPlayer
{
    public class Player
    {
        #region base
        private readonly NAudioCore nAudioCore;
        public readonly PlaylistV2 Playlist;

        public Player()
        {
            nAudioCore = new(this);
            Playlist = new(this);
        }
        public Player(PlaylistV2 playlistV2)
        {
            nAudioCore = new(this);
            this.Playlist = playlistV2;
        }
        #endregion

        #region Void
        public void Play() => nAudioCore.Play();
        public void Pause() => nAudioCore.Pause();
        public void Close() => nAudioCore.Close();
        public void Stop() => nAudioCore.Stop();
        public void Seek(double value) => nAudioCore.Seek(value);

        #endregion

        #region Async
        public async Task OpenAsync() => await nAudioCore.OpenAsync();
        public async Task SeekAsync(double value) => await nAudioCore.SeekAsync(value);

        #endregion

        #region get and set
        public string Source { get => nAudioCore.Source; set => nAudioCore.Source = value; }
        public TimeSpan TimePosition { get => nAudioCore.CurrentTime; set => nAudioCore.CurrentTime = value; }

        #endregion

        #region get
        public TimeSpan TimeDuration => nAudioCore.TotalTime;
        public PlaybackState PlaybackState => nAudioCore.PlaybackState;

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

        public int Volume { get => nAudioCore.Volume; private set => nAudioCore.Volume = value; }
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
        public void ChangeEq(int bandIndex, float Gain) => nAudioCore.ChangeEqualizerBand(bandIndex, Gain);
        public double GetEqBandGain(int BandIndex) => nAudioCore.GetEqBandGain(BandIndex);

        #endregion
    }
}
