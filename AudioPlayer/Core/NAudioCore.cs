using NAudio.Extras;
using NAudio.Wave;
using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace AudioPlayer.Core
{
    //ToDo: EngineMode  like with eq or with mono or pitch changable
    //ToDo Reader.WaveFormat.BitsPerSample
    internal class NAudioCore
    {
        #region NAudio Engine
        private MediaFoundationReader Reader;
        private ISampleProvider SampleProvider => Reader.ToSampleProvider();
        private Equalizer EqualizerCore;
        private readonly WaveOutEvent WaveOutEvent = new();

        private readonly EqualizerMode equalizerMode = EqualizerMode.NormalEqualizer8band;
        private EqualizerBand[] EqualizerBand { get; set; }
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

        private void InitalEqualizer()
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

        internal void Seek(double totalseconds)
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

        internal class EngineInfo
        {
            public int SampleRate;
            public int Channels;
            public int AverageBytesPerSecond;
            public WaveFormatEncoding Encoding;
            public int BitsPerSample;

            public EngineInfo(MediaFoundationReader reader)
            {
                SampleRate = reader.WaveFormat.SampleRate;
                Channels = reader.WaveFormat.Channels;
                AverageBytesPerSecond = reader.WaveFormat.AverageBytesPerSecond;
                Encoding = reader.WaveFormat.Encoding;
                BitsPerSample = reader.WaveFormat.BitsPerSample;
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override string ToString()
            {
                base.ToString();
                return $"Media Info <SampleRate: {SampleRate}> | <Channels: {Channels}> | <avgBPM {AverageBytesPerSecond}> | <Encoding: {Encoding}> | <BPM: {BitsPerSample}>";
            }
        }

        internal async Task OpenAsync()
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
                    EqualizerCore = new(SampleProvider, EqualizerBand);
                    WaveOutEvent.Init(EqualizerCore);

                    PlaybackState = PlaybackState.Opened;
                    Play();
                    Log.WriteLine(new EngineInfo(Reader).ToString());
                }
                catch (Exception ex)
                {
                    Log.WriteLine(ex.Message);
                }

            });

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
            Seek(0);
            PlaybackState = PlaybackState.Stopped;
        }

        internal void Close()
        {
            WaveOutEvent.Dispose();
            Reader.Close();
            PlaybackState = PlaybackState.Closed;
        }

        internal async Task SeekAsync(double totalseconds)
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

        private PlaybackState _playbackState;
        internal PlaybackState PlaybackState
        {
            get => _playbackState;
            set
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
            if ((int)Reader.CurrentTime.TotalSeconds >= (int)Reader.TotalTime.TotalSeconds)
            {
                PlaybackState = PlaybackState.Ended;
            }
            GC.Collect();
        }

        internal float ToSingle(double value)
        {
            return (float)value;
        }
        internal double ToDouble(float value)
        {
            return value;
        }

        private void WaveOutEvent_PlaybackStopped(object sender, StoppedEventArgs e)
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
        private readonly DispatcherTimer CurrentTimeWatcher = new();
        internal const string stringformat = "mm\\:ss";
    }
}
