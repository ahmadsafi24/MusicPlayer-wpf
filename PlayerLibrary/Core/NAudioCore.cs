using NAudio.Extras;
using NAudio.Wave;
using PlayerLibrary.Model;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PlayerLibrary.Core
{
    //ToDo: EngineMode  like with eq or with mono or pitch changable
    internal class NAudioCore
    {
        #region NAudio Engine
        private MediaFoundationReader Reader;
        private ISampleProvider SampleProvider => Reader.ToSampleProvider();
        private Equalizer EqualizerCore;
        private readonly WaveOutEvent WaveOutEvent = new();

        internal EqualizerMode equalizerMode = EqualizerMode.Super;
        internal EqualizerBand[] EqualizerBand { get; set; }
        #endregion

        private readonly Player PublicPlayer;
        #region Initial
        internal NAudioCore(Player player)
        {
            PublicPlayer = player;
            InitalEqualizer();
            InitializeTimers();

            WaveOutEvent.PlaybackStopped += WaveOutEvent_PlaybackStopped;
            Log.WriteLine("PlayerInitialized");
            PlaybackState = PlaybackState.Closed;

        }

        private void InitializeTimers()
        {
            CurrentTimeWatcher.Interval = TimeSpan.FromMilliseconds(200);
            CurrentTimeWatcher.Tick += CurrentTimeWatcher_Tick;
        }

        internal void InitalEqualizer()
        {
            switch (equalizerMode)
            {
                case EqualizerMode.Normal:
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
                case EqualizerMode.Super:
                    EqualizerBand = new EqualizerBand[]
                    {
                        new EqualizerBand { Bandwidth = 0.4f, Frequency = 30, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.4f, Frequency = 50, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.4f, Frequency = 90, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.4f, Frequency = 160, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.4f, Frequency = 300, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.4f, Frequency = 500, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.4f, Frequency = 1000, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.4f, Frequency = 1600, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.4f, Frequency = 3000, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.4f, Frequency = 5000, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.4f, Frequency = 9000, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.4f, Frequency = 16000, Gain = 0 },
                    };
                    break;
                case EqualizerMode.Disabled:
                    EqualizerBand = Array.Empty<EqualizerBand>();
                    break;
                default:
                    break;
            }
        }

        internal void Open(string filePath, TimeSpan timePosition)
        {
            Open(filePath);
            Seek(timePosition);
        }
        #endregion

        private string source;
        internal string Source { get => source; set { source = value; Log.WriteLine($"Source: {value}"); } }

        internal double CurrentTimeWatcherInterval
        {
            get => CurrentTimeWatcher.Interval.TotalSeconds;
            set => CurrentTimeWatcher.Interval = TimeSpan.FromSeconds(value);
        }

        internal int Volume
        {
            get
            {
                return (int)(ToDouble(WaveOutEvent.Volume) * 100);
            }
            set
            {
                try
                {
                    if (IsMuted)
                    {
                        ismute = false;
                    }
                    int iv = value < 0 ? 0 : value > 100 ? 100 : value;
                    double V = (double)iv / 100;
                    //V = V < 0 ? 0 : V > 1 ? 1 : V;
                    WaveOutEvent.Volume = (float)V;

                    if (iv == 0)
                    {
                        ismute = true;
                    }
                    PublicPlayer.InvokeVolumeChanged(iv);
                    Log.WriteLine("volume: " + iv);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(ex.Message);
                }
            }
        }

        internal void Seek(TimeSpan timePosition)
        {
            try
            {
                CurrentTime = timePosition;
                Log.WriteLine($"Seeking to {timePosition.ToString(stringformat)}");
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message);
            }
        }

        internal TimeSpan CurrentTime
        {
            get => Reader is null ? TimeSpan.FromSeconds(0) : Reader.CurrentTime;

            set
            {
                if (Reader is not null)
                {
                    Reader.CurrentTime = value.TotalSeconds <= 0 ? TimeSpan.FromSeconds(0) : value;
                    PublicPlayer.InvokeCurrentTime(value);
                }
            }
        }

        internal TimeSpan TotalTime => Reader is null ? TimeSpan.FromSeconds(0) : Reader.TotalTime;

        internal ReaderInfo ReaderInfo;
        internal async Task OpenAsync(string filePath)
        {
            await Task.Run(() =>
            {
                Open(filePath);
            });

        }

        internal void Open(string filePath)
        {
            try
            {
                if (PlaybackState is PlaybackState.Playing or PlaybackState.Paused)
                {
                    Stop();
                    Close();
                }
                Source = filePath;
                if (string.IsNullOrEmpty(Source))
                {
                    MessageBox.Show("Core: Empty Source");
                    return;
                }
                Reader = new MediaFoundationReader(Source);
                if (equalizerMode == EqualizerMode.Disabled)
                {
                    WaveOutEvent.Init(Reader);
                }
                else
                {
                    EqualizerCore = new(SampleProvider, EqualizerBand);
                    WaveOutEvent.Init(EqualizerCore);
                }

                PlaybackState = PlaybackState.Opened;
                ReaderInfo = new(Reader);
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message);
            }
        }

        internal void Play()
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

        internal void Pause()
        {
            WaveOutEvent.Pause();
            PlaybackState = PlaybackState.Paused;
        }

        internal void Stop()
        {
            WaveOutEvent.Stop();
            Seek(TimeSpan.Zero);
            PlaybackState = PlaybackState.Stopped;
        }

        internal void Close()
        {
            WaveOutEvent.Dispose();
            Reader.Close();
            Source = null;
            Reader.Dispose();
            Reader = null;
            PlaybackState = PlaybackState.Closed;
        }

        internal async Task SeekAsync(TimeSpan time)
        {
            try
            {
                await Task.Run(() =>
                {
                    CurrentTime = time;
                    Log.WriteLine($"Seeking (async) to {time.ToString(stringformat)}");
                });
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message);
            }
        }

        internal void ChangeEqualizerBand(int bandIndex, float gain)
        {
            EqualizerBand[bandIndex].Gain = gain;
            EqualizerCore?.Update();
        }

        internal double GetEqBandGain(int bandindex)
        {
            return EqualizerBand[bandindex].Gain;
        }

        internal void ChangeEqualizerBand(int bandIndex, float Gain, float Bandwidth, float Frequency)
        {
            if (Gain != 0) { EqualizerBand[bandIndex].Gain = Gain; }
            if (Bandwidth != 0) { EqualizerBand[bandIndex].Bandwidth = Bandwidth; }
            if (Frequency != 0) { EqualizerBand[bandIndex].Frequency = Frequency; }
            EqualizerCore?.Update();
        }

        internal void ResetEq()
        {
            foreach (var item in EqualizerBand)
            {
                item.Gain = 0;
            }
            EqualizerCore?.Update();
        }

        // index: band position
        // double:gain
        internal double[] Bands8
        {
            get
            {
                double[] bands = Array.Empty<double>();
                int i = 0;
                foreach (var item in EqualizerBand)
                {
                    bands.SetValue(item.Gain, i);
                    i++;
                }
                return bands;
            }
        }

        private PlaybackState _playbackState;
        internal PlaybackState PlaybackState
        {
            get => _playbackState;
            private set
            {
                Log.WriteLine("PlaybackState: " + value.ToString());
                _playbackState = value;
                PublicPlayer.InvokePlaybackStateChanged(value);
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

        private int volBeforeMute;
        private bool ismute;
        internal bool IsMuted
        {
            get => ismute;
            set
            {
                if (value)
                {
                    volBeforeMute = Volume;
                    Volume = 0;
                }
                else
                {
                    Volume = volBeforeMute;
                }
                ismute = value;
            }
        }

        private void CurrentTimeWatcher_Tick(object sender, EventArgs e)
        {
            PublicPlayer.InvokeCurrentTime(Reader.CurrentTime);

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

        private void WaveOutEvent_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Exception?.Message))
            {
                if ((int)Reader.CurrentTime.TotalSeconds >= (int)Reader.TotalTime.TotalSeconds - 1)
                {
                    PlaybackState = PlaybackState.Ended;
                }
            }
            else
            {
                Log.WriteLine(e.Exception.Message + "WaveOutEvent_PlaybackStopped");
                PlaybackState = PlaybackState.Failed;
            }
        }
        private readonly DispatcherTimer CurrentTimeWatcher = new();
        internal const string stringformat = "mm\\:ss";
    }
}
