using Engine.Enums;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Engine
{
    public static class Player
    {
        public static event EventHandlerPlaybackState PlaybackStateChanged;
        public static event EventHandlerTimeSpan CurrentTimeChanged;
        public static event EventHandlerVolume VolumeChanged;

        internal static void InvokePlaybackStateChanged(PlaybackState value) => PlaybackStateChanged?.Invoke(value);
        internal static async void InvokeCurrentTime(TimeSpan timespan) => await Task.Run(() => CurrentTimeChanged?.Invoke(timespan));
        internal static void InvokeVolumeChanged(int newVolume)
        {
            Log.WriteLine("volume: " + newVolume);
            VolumeChanged?.Invoke(newVolume);
        }

        public static string Source { get => Internal.NaudioPlayer.Source; set => Internal.NaudioPlayer.Source = value; }
        public static int Volume { get => Internal.NaudioPlayer.Volume; private set => Internal.NaudioPlayer.Volume = value; }
        public static TimeSpan CurrentTime => Internal.NaudioPlayer.CurrentTime;
        public static TimeSpan TotalTime => Internal.NaudioPlayer.TotalTime;
        public static PlaybackState PlaybackState => Internal.NaudioPlayer.PlaybackState;

        public static void Initialize()
        {
            Internal.NaudioPlayer.Initialize();
        }

        public static async Task OpenAsync() => await Internal.NaudioPlayer.OpenAsync();
        public static void Play() => Internal.NaudioPlayer.Play();
        public static void Pause() => Internal.NaudioPlayer.Pause();
        public static void Close() => Internal.NaudioPlayer.Close();
        public static void Stop() => Internal.NaudioPlayer.Stop();

        public static async Task SeekAsync(double value) => await Internal.NaudioPlayer.SeekAsync(value);
        public static void Seek(double value) => Internal.NaudioPlayer.Seek(value);

        public static void VolumeUp(int value) => ChangeVolume(Internal.NaudioPlayer.Volume += value);
        public static void VolumeDown(int value) => ChangeVolume(Internal.NaudioPlayer.Volume -= value);
        public static void ChangeVolume(int newVolume)
        {
            try
            {
                if (newVolume is >= 0 and <= 100)
                {
                    Internal.NaudioPlayer.Volume = newVolume;
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message);
            }
        }

        public static double GetBandGain(int BandIndex)
        {
            return Internal.NaudioPlayer.GetEqBandGain(BandIndex);
        }

        public static void ResetEq()
        {
            Internal.NaudioPlayer.ResetEq();
        }

        public static void ChangeEq(int bandIndex, float Gain)
        {
            Internal.NaudioPlayer.ChangeEqualizerBand(bandIndex, Gain);
        }

        public static void FindCurrentFile()
        {
            PlaylistManager.FindOpenedFileIndex();
        }
    }
}
