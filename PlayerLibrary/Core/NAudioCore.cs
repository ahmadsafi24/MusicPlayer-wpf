using NAudio.Extras;
using NAudio.Wave;
using System;
using System.Collections.Generic;

namespace PlayerLibrary.Core
{
    //TODO: EngineMode  like with eq or with mono or pitch changable
    internal class NAudioCore
    {
        #region NAudio Engine
        internal MediaFoundationReader Reader;
        internal ISampleProvider SampleProvider => Reader.ToSampleProvider();
        internal Equalizer EqualizerCore;
        internal WaveOutEvent WaveOutEvent = new();
        internal EqualizerMode equalizerMode { get; set; } = EqualizerMode.Super;
        internal EqualizerBand[] EqualizerBand { get; set; }
        #endregion

        private readonly Player publicPlayer;
        #region Initial
        internal NAudioCore(Player player)
        {
            publicPlayer = player;
            InternalInitialize();
        }

        internal NAudioCore()
        {
            InternalInitialize();
        }

        private void InternalInitialize()
        {
            InitalEqualizer();


            //PlaybackState = PlaybackState.Closed;
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


        #endregion

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
                    publicPlayer.InvokeVolumeChanged(iv);
                    Log.WriteLine("volume: " + iv);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(ex.Message);
                }
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
        internal int[] BandsGain
        {
            get
            {
                List<int> GainList = new();
                foreach (var item in EqualizerBand)
                {
                    GainList.Add((int)item.Gain);
                }
                return GainList.ToArray();
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



        internal static float ToSingle(double value)
        {
            return (float)value;
        }
        internal static double ToDouble(float value)
        {
            return value;
        }

        internal void ReIntialEq()
        {
            //PlaybackState lastpstate = PlaybackState;
            //ToggleEventsOff();

            InitalEqualizer();
            //if (!string.IsNullOrEmpty(Source))
            {
                //Open(Source, Reader.CurrentTime);
                /*switch (lastpstate)
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
                }*/
            }

            //ToggleEventsOn();
            publicPlayer.FireEqUpdated();
        }

        internal void ChangeEq(int bandIndex, float Gain, bool requestNotifyEqUpdate)
        {
            ChangeEqualizerBand(bandIndex, Gain);
            if (requestNotifyEqUpdate)
            {
                publicPlayer.FireEqUpdated();
            }
        }

        internal void ChangeBands(int[] Bands)
        {
            EqualizerMode newEqMode = new();
            if (Bands.Length == 8)
            {
                newEqMode = EqualizerMode.Normal;
            }
            else if (Bands.Length == 12)
            {
                newEqMode = EqualizerMode.Super;
            }
            if (equalizerMode != newEqMode)
            {
                equalizerMode = newEqMode;
                ReIntialEq();
            }
            int i = 0;
            foreach (var item in Bands)
            {
                ChangeEqualizerBand(i, (float)item);
                publicPlayer.FireEqUpdated();
                i++;
            }
        }

    }
}
