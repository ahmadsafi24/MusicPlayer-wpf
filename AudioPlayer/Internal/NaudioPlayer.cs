using Engine.Enums;
using NAudio.Extras;
using NAudio.Wave;
using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using PlaybackState = Engine.Enums.PlaybackState;

namespace Engine.Internal
{
    //ToDo: EngineMode  like with eq or with mono or pitch changable
    //ToDo Reader.WaveFormat.BitsPerSample
    internal sealed class NaudioPlayer
    {
        #region NAudio Engine
        private static MediaFoundationReader Reader;
        private static ISampleProvider SampleProvider => Reader.ToSampleProvider();
        private static Equalizer Equalizer8band => new(SampleProvider, EqualizerBand);
        private static WaveOutEvent WaveOutEvent = new();

        private static EqualizerMode equalizerMode = EqualizerMode.NormalEqualizer8band;
        private static EqualizerBand[] EqualizerBand { get; set; }
        #endregion

        #region Initial
        internal static void Initialize()
        {
            InitalEqualizer();
            InitializeTimers();

            WaveOutEvent.PlaybackStopped += WaveOutEvent_PlaybackStopped;
            Log.WriteLine("PlayerInitialized");
            PlaybackState = PlaybackState.Closed;
        }

        private static void InitializeTimers()
        {
            CurrentTimeWatcher.Interval = TimeSpan.FromMilliseconds(200);
            CurrentTimeWatcher.Tick += CurrentTimeWatcher_Tick;
        }

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
        #endregion

        private static string source;
        internal static string Source { get => source; set { source = value; Log.WriteLine($"Source: {value}"); } }

        internal static double CurrentTimeWatcherInterval
        {
            get => CurrentTimeWatcher.Interval.TotalSeconds;
            set => CurrentTimeWatcher.Interval = TimeSpan.FromSeconds(value);
        }

        internal static void ChangeEqualizerBand(int bandIndex, float gain)
        {
            EqualizerBand[bandIndex].Gain = gain;
            Equalizer8band.Update();
        }

        internal static int Volume
        {
            get
            {
                return (int)(ToDouble(WaveOutEvent.Volume) * 100);
            }
            set
            {
                try
                {
                    int iv = value < 0 ? 0 : value > 100 ? 100 : value;
                    double V = (double)iv / 100;
                    //V = V < 0 ? 0 : V > 1 ? 1 : V;
                    WaveOutEvent.Volume = (float)V;
                    Player.InvokeVolumeChanged(iv);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(ex.Message);
                }
            }
        }

        internal static void Seek(double totalseconds)
        {
            try
            {
                CurrentTime = TimeSpan.FromSeconds(totalseconds);
                Log.WriteLine($"Seeking to {TimeSpan.FromSeconds(totalseconds).ToString(stringformat)}");
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message);
            }
        }

        internal static TimeSpan CurrentTime
        {
            get => Reader is null ? TimeSpan.FromSeconds(0) : Reader.CurrentTime;

            private set
            {
                if (Reader is not null)
                {
                    Reader.CurrentTime = value.TotalSeconds <= 0 ? TimeSpan.FromSeconds(0) : value;
                    Player.InvokeCurrentTime(value);
                }
            }
        }

        internal static TimeSpan TotalTime => Reader is null ? TimeSpan.FromSeconds(0) : Reader.TotalTime;


        internal static async Task OpenAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    if (PlaybackState is PlaybackState.Playing or PlaybackState.Paused)
                    {
                        Stop();
                        Close();
                    }

                    Reader = new MediaFoundationReader(Source);
                    WaveOutEvent.Init(Equalizer8band);

                    //PlaylistManager.FindOpenedFileIndex();
                    PlaybackState = PlaybackState.Opened;
                    Play();
                }
                catch (Exception ex)
                {
                    Log.WriteLine(ex.Message);
                }

            });

        }

        internal static void Play()
        {
            try
            {
                if (PlaybackState is PlaybackState.Closed)
                {
                    return;
                }
                WaveOutEvent.Play();
                PlaybackState = PlaybackState.Playing;
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex);
            }
        }

        internal static void Pause()
        {
            WaveOutEvent.Pause();
            PlaybackState = PlaybackState.Paused;
        }

        internal static void Stop()
        {
            WaveOutEvent.Stop();
            Seek(0);
            PlaybackState = PlaybackState.Stopped;
        }

        internal static void Close()
        {
            WaveOutEvent.Dispose();
            Reader.Close();
            PlaybackState = PlaybackState.Closed;
        }

        internal static async Task SeekAsync(double totalseconds)
        {
            try
            {
                await Task.Run(() =>
                {
                    CurrentTime = TimeSpan.FromSeconds(totalseconds);
                    Log.WriteLine($"Seeking (async) to {TimeSpan.FromSeconds(totalseconds).ToString(stringformat)}");
                });
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message);
            }
        }

        internal static double GetEqBandGain(int bandindex)
        {
            return EqualizerBand[bandindex].Gain;
        }

        internal static void ChangeEqualizerBand(int bandIndex, float Gain, float Bandwidth, float Frequency)
        {
            if (Gain != 0) { EqualizerBand[bandIndex].Gain = Gain; }
            if (Bandwidth != 0) { EqualizerBand[bandIndex].Bandwidth = Bandwidth; }
            if (Frequency != 0) { EqualizerBand[bandIndex].Frequency = Frequency; }
            Equalizer8band.Update();
        }

        internal static void ResetEq()
        {
            foreach (var item in EqualizerBand)
            {
                item.Gain = 0;
            }
            Equalizer8band.Update();
        }

        private static PlaybackState _playbackState;
        internal static PlaybackState PlaybackState
        {
            get => _playbackState;
            set
            {
                Log.WriteLine("PlaybackState: " + value.ToString());
                _playbackState = value;
                Player.InvokePlaybackStateChanged(value);
                if (value is PlaybackState.Playing)
                {
                    CurrentTimeWatcher.Start();
                }
                else
                {
                    CurrentTimeWatcher.Stop();
                }
            }
        }

        private static void CurrentTimeWatcher_Tick(object sender, EventArgs e)
        {
            Player.InvokeCurrentTime(Reader.CurrentTime);
            if ((int)Reader.CurrentTime.TotalSeconds >= (int)Reader.TotalTime.TotalSeconds)
            {
                PlaybackState = PlaybackState.Ended;
            }
            GC.Collect();
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
                //Stop();
            }
            else
            {
                Log.WriteLine(e.Exception.Message + "WaveOutEvent_PlaybackStopped");
                PlaybackState = PlaybackState.Failed;
            }
        }
        private static readonly DispatcherTimer CurrentTimeWatcher = new();
        internal const string stringformat = "mm\\:ss";
    }
}
