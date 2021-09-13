using System;
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
    internal static class Player
    {
        #region Root

        private static MediaFoundationReader Reader;
        private static WaveOutEvent WaveOutEvent = new();
        private static Equalizer Equalizer8band;
        private static ISampleProvider SampleProvider;
        private static readonly DispatcherTimer CurrentTimeWatcher = new();
        private static readonly EqualizerMode equalizerMode = EqualizerMode.NormalEqualizer8band;
        internal static RepeatMode RepeatMode { get; set; } = RepeatMode.CurrentFile;
        internal static EqualizerBand[] EqualizerBand { get; set; }

        private static string source;
        internal static string Source { get => source; set { source = value; OpenFile(value); } }

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
            WaveOutEvent.PlaybackStopped += WaveOutEvent_PlaybackStopped;
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
                    Close();

                    WaveOutEvent = new WaveOutEvent();
                    Reader = new MediaFoundationReader(File);
                    SampleProvider = Reader.ToSampleProvider();
                    Equalizer8band = new Equalizer(SampleProvider, EqualizerBand);
                    WaveOutEvent.Init(Equalizer8band);

                    /*{//test for no eq
                        WaveOutEvent.Init(new AudioFileReader(File));
                    }*/

                    PlaylistManager.CurrentFileTag = new(File);
                    CurrentPlaybackState = PlaybackState.Opened;
                    Play();
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show(ex.Message);
                    throw;
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
            string f = await Helper.Utility.FileOpenPicker.GetFileAsync();
            if (System.IO.File.Exists(f))
            {
                await OpenFileAsync(f);
            }
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
                _CurrentPlaybackState = value;
                Events.AllEvents.InvokePlaybackStateChanged(value);
                switch (value)
                {
                    case PlaybackState.Unknown:
                        break;
                    case PlaybackState.Failed:
                        CurrentTimeWatcher.Stop();
                        break;
                    case PlaybackState.Opened:
                        break;
                    case PlaybackState.Paused:
                        break;
                    case PlaybackState.Playing:
                        CurrentTimeWatcher.Start();
                        break;
                    case PlaybackState.Stopped:
                        break;
                    case PlaybackState.Ended:
                        OnPlaybackEnded();
                        break;
                    case PlaybackState.Closed:
                        CurrentTimeWatcher.Stop();
                        break;
                    default:
                        break;
                }

            }
        }

        private static void OnPlaybackEnded()
        {
            switch (RepeatMode)
            {
                case RepeatMode.Stop:
                    Stop();
                    break;
                case RepeatMode.Close:
                    Close();
                    break;
                case RepeatMode.CurrentFile:
                    Stop();
                    CurrentTime = TimeSpan.FromSeconds(0);
                    Play();
                    break;
                case RepeatMode.NextFile:
                    break;
                case RepeatMode.CurrentPlaylist:
                    break;
                default:
                    break;
            }

        }

        private static void CurrentTimeWatcher_Tick(object sender, EventArgs e)
        {
            Events.AllEvents.InvokeCurrentTime();

            if ((int)CurrentTime.TotalSeconds >= (int)TotalTime.TotalSeconds)
            {
                Task.Delay(1000).Wait();
                CurrentPlaybackState = PlaybackState.Ended;
            }
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
                        new EqualizerBand { Bandwidth = 0.8f, Frequency = 100, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.8f, Frequency = 200, Gain = 0 },
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
            if (string.IsNullOrEmpty(e.Exception.Message))
            {
                CurrentPlaybackState = PlaybackState.Stopped;
            }
            _ = MessageBox.Show(e.Exception.Message);
        }
        #endregion

        internal static void Dispose()
        {
            Close();
            GC.Collect();
        }

        internal static async void Open()
        {
            await OpenFileAsync(Source);
        }
    }
}
