using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Engine.Enums;
using NAudio.Extras;
using NAudio.Wave;
using PlaybackState = Engine.Enums.PlaybackState;

namespace Engine.Internal
{
    //ToDo Reader.WaveFormat.BitsPerSample
    internal sealed class Player
    {
        #region Root

        private static MediaFoundationReader Reader;
        private static ISampleProvider SampleProvider => Reader.ToSampleProvider();
        private static Equalizer Equalizer8band => new(SampleProvider, EqualizerBand);
        private static WaveOutEvent WaveOutEvent = new();

        private static readonly DispatcherTimer CurrentTimeWatcher = new();
        private static readonly EqualizerMode equalizerMode = EqualizerMode.NormalEqualizer8band;
        internal static EqualizerBand[] EqualizerBand { get; set; }

        private static string source;
        internal static string Source { get => source; set { source = value; OpenFile(value); Debug.WriteLine(value); } }

        private static async void OpenFile(string value)
        {
            await OpenFileAsync(value);
        }

        internal static string stringformat = "mm\\:ss";
        internal static void Initialize()//todo: EngineMode  like with eq or with mono or pitch changable
        {
            InitalEqualizer();
            CurrentTimeWatcher.Interval = TimeSpan.FromMilliseconds(100);
            CurrentTimeWatcher.Tick += CurrentTimeWatcher_Tick;
            WaveOutEvent = new WaveOutEvent();
            WaveOutEvent.PlaybackStopped += WaveOutEvent_PlaybackStopped;

            Debug.WriteLine("PlayerInitialized");
        }

        #endregion

        #region ControlPlayer
        internal static TimeSpan CurrentTimeWatcherInterval
        {
            get => CurrentTimeWatcher.Interval;
            set => CurrentTimeWatcher.Interval = value;
        }


        internal static double Volume
        {
            get => ToDouble(WaveOutEvent.Volume);
            set
            {
                try
                {
                    WaveOutEvent.Volume = value < 0 ? ToSingle(0) : value > 1 ? ToSingle(1) : ToSingle(value);
                    Events.AllEvents.InvokeVolumeChanged();
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show(ex.Message);
                }
            }
        }

        internal static void Seek(double totalseconds)
        {
            try
            {
                CurrentTime = TimeSpan.FromSeconds(totalseconds);
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message);
            }
        }

        internal static TimeSpan CurrentTime
        {
            get => Reader is null ? TimeSpan.FromSeconds(0) : Reader.CurrentTime;

            set
            {
                if (Reader is not null)
                {
                    Reader.CurrentTime = value.TotalSeconds <= 0 ? TimeSpan.FromSeconds(0) : value;
                    Events.AllEvents.InvokeCurrentTime();
                }
            }
        }

        internal static TimeSpan TotalTime => Reader is null ? TimeSpan.FromSeconds(0) : Reader.TotalTime;

        internal static string CurrentTimeString => CurrentTime.ToString(stringformat);

        internal static string TotalTimeString => TotalTime.ToString(stringformat);
        #endregion

        #region Tasks

        private static async Task OpenFileAsync(string File)
        {
            await Task.Run(() =>
            {
                try
                {
                    Stop();

                    Reader = new MediaFoundationReader(File);
                    WaveOutEvent.Init(Equalizer8band);

                    /*{//test for no eq
                        WaveOutEvent.Init(new AudioFileReader(File));
                    }*/

                    PlaylistManager.FindOpenedFileIndex();
                    CurrentPlaybackState = PlaybackState.Opened;
                    Play();
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show(ex.Message);
                }

            });

        }


        internal static void PlayPause()
        {
            if (CurrentPlaybackState == PlaybackState.Playing)
            {
                Pause();
            }
            else if (CurrentPlaybackState is PlaybackState.Paused
                    or PlaybackState.Stopped
                    or PlaybackState.Ended)
            {
                Play();
            }
            else
            {
                OpenFilePicker();
            }
        }

        internal static async void OpenFilePicker()
        {
            string[] files = await Helper.Utility.FileOpenPicker.GetFileAsync();
            foreach (var file in files)
            {
                if (System.IO.File.Exists(file))
                {
                    PlaylistManager.Add(0, file);
                }
            }
            Source = files[0];
        }

        internal static void Play()
        {
            WaveOutEvent.Play();
            CurrentPlaybackState = PlaybackState.Playing;
        }

        internal static void Pause()
        {
            WaveOutEvent.Pause();
            CurrentPlaybackState = PlaybackState.Paused;
        }

        internal static void Stop()
        {
            WaveOutEvent.Stop();
            CurrentPlaybackState = PlaybackState.Stopped;
        }

        internal static void Close()
        {
            WaveOutEvent?.Dispose();
            WaveOutEvent = null;
            Reader?.Dispose();
            Reader = null;
            CurrentPlaybackState = PlaybackState.Closed;
        }

        internal static async Task SeekAsync(double totalseconds)
        {
            try
            {
                await Task.Run(() =>
                {
                    CurrentTime = TimeSpan.FromSeconds(totalseconds);
                });
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message);
            }
        }

        internal static void ChangeEqualizerBand(int bandIndex, float Gain, float Bandwidth, float Frequency)
        {
            if (Gain != 0) { EqualizerBand[bandIndex].Gain = Gain; }
            if (Bandwidth != 0) { EqualizerBand[bandIndex].Bandwidth = Bandwidth; }
            if (Frequency != 0) { EqualizerBand[bandIndex].Frequency = Frequency; }
            Equalizer8band.Update();
        }

        #endregion

        #region sec1


        private static PlaybackState _CurrentPlaybackState;

        internal static PlaybackState CurrentPlaybackState
        {
            get => _CurrentPlaybackState;
            set
            {
                Debug.WriteLine(value.ToString());
                _CurrentPlaybackState = value;
                Events.AllEvents.InvokePlaybackStateChanged(value);
                if (value is PlaybackState.Playing)
                {
                    CurrentTimeWatcher.Start();
                }
                else if (value == PlaybackState.Ended)
                {
                    PlaylistManager.PlaybackEnded();
                }
                else
                {
                    CurrentTimeWatcher.Stop();
                }
            }
        }

        private static void CurrentTimeWatcher_Tick(object sender, EventArgs e)
        {
            Events.AllEvents.InvokeCurrentTime();

            if ((int)CurrentTime.TotalSeconds >= (int)TotalTime.TotalSeconds)
            {
                CurrentPlaybackState = PlaybackState.Ended;
            }
            GC.Collect();
        }

        #endregion

        #region Internal

        private static void InitalEqualizer()
        {
            switch (equalizerMode)
            {
                case EqualizerMode.NormalEqualizer8band:
                    EqualizerBand = new EqualizerBand[]
                    {
                        new EqualizerBand { Bandwidth = 0.8f, Frequency = 100, Gain = 10 },
                        new EqualizerBand { Bandwidth = 0.8f, Frequency = 200, Gain = 10 },
                        new EqualizerBand { Bandwidth = 0.8f, Frequency = 400, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.8f, Frequency = 800, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.8f, Frequency = 1200, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.8f, Frequency = 2400, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.8f, Frequency = 4800, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.8f, Frequency = 9600, Gain = 0 },
                    };
                    break;
                case EqualizerMode.SuperEqualizer16band:
                    break;
                default:
                    break;
            }
        }
        internal static float ToSingle(double value)
        {
            return (float)value;
        }
        internal static double ToDouble(float value)
        {
            return value;
        }
        private static void WaveOutEvent_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Exception?.Message))
            {
                CurrentPlaybackState = PlaybackState.Stopped;
            }
            else
            {
                _ = MessageBox.Show(e.Exception.Message);
                CurrentPlaybackState = PlaybackState.Failed;
            }
        }
        #endregion

        internal static void Dispose()
        {
            Close();
        }

    }
}
