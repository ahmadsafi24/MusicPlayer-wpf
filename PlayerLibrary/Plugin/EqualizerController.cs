using Helper;
using NAudio.Extras;
using NAudio.Wave;
using PlayerLibrary.Core;
using PlayerLibrary.Model;
using System;
using System.Collections.Generic;
using static PlayerLibrary.Events;

namespace PlayerLibrary.Plugin
{
    public class EqualizerController : IPlugin
    {
        #region fields
        private readonly PlaybackSession PlaybackSession;

        #endregion
        EqualizerBand[] bands = Create8Band();
        Equalizer EqualizerCore;
        private void InitEqualizerSamleProvider()
        {
            EqualizerCore = new(PlaybackSession.audioPlayer.Reader.ToSampleProvider(), bands);
            PlaybackSession.audioPlayer.SampleProvider = EqualizerCore;

        }
        #region property
        private EqualizerMode _equalizerMode;
        public EqualizerMode EqualizerMode
        {
            get => _equalizerMode;
            set => _equalizerMode = value;

        }

        /// <summary>
        /// get all bands gain in single array: array length = bands count ,int array[index] = int Allbands[index]
        /// </summary>
        /// <value></value>
        public int[] AllBandsGain => GetBandsGain(bands);

        #endregion


        public void SetEqPreset(EqPreset preset)
        {
            EqualizerMode mode = (EqualizerMode)Enum.Parse(typeof(EqualizerMode), preset.EqualizerMode);
            if (mode != EqualizerMode)
            {
                EqualizerMode = mode;
                ResetEqController(mode, PlaybackSession);
            }
            SetAllBands(preset.BandsGain);
        }

        public EqPreset GetEqPreset()
        {
            EqPreset preset = new(EqualizerMode.ToString(), AllBandsGain);
            return preset;
        }

        #region Event
        public EventHandlerEmpty EqUpdated;
        internal void FireEqUpdated()
        {
            EqUpdated?.Invoke();
        }

        #endregion

        /// <summary>
        /// Change a single band gain 
        /// </summary>
        /// <param name="bandIndex"> band index</param>
        /// <param name="gain"> band gain</param>
        internal void SetBandGain(int bandIndex, float gain)
        {
            if (bandIndex > bands.Length)
            {
                Helper.Log.WriteLine("band index bigger then equalizerband length");
                return;
            }
            else
            {
                Log.WriteLine("changing band index:" + bandIndex.ToString());
                bands[bandIndex].Gain = gain;
                EqualizerCore?.Update();
            }
        }

        public double GetBandGain(int bandindex)
        {
            if (bands == null)
            {
                return 0;
            }
            if (bands.Length <= bandindex)
            {
                return 0;
            }
            else
            {
                return bands[bandindex].Gain;
            }
        }

        internal void SetBand(int bandIndex, float Gain, float Bandwidth, float Frequency)
        {
            if (bands == null)
            {
                return;
            }

            if (Gain != 0) { bands[bandIndex].Gain = Gain; }
            if (Bandwidth != 0) { bands[bandIndex].Bandwidth = Bandwidth; }
            if (Frequency != 0) { bands[bandIndex].Frequency = Frequency; }
            EqualizerCore?.Update();
        }

        public void SetAllBandsGain(int gain)
        {
            foreach (var item in bands)
            {
                item.Gain = 0;
            }
            EqualizerCore?.Update();
            FireEqUpdated();
        }

        public void SetBandGain(int bandIndex, float Gain, bool requestNotifyEqUpdate)
        {
            try
            {
                SetBandGain(bandIndex, Gain);
                if (requestNotifyEqUpdate)
                {
                    FireEqUpdated();
                }
            }
            catch (System.Exception ex)
            {
                Helper.Log.WriteLine("SetBandGain", ex.Message);
                throw;
            }
        }

        public void SetAllBands(int[] Bands)
        {
            int i = 0;
            foreach (var item in Bands)
            {
                SetBandGain(i, (float)item);
                FireEqUpdated();
                i++;
            }
        }

        public void RequestResetEqController() { ResetEqController(EqualizerMode, PlaybackSession); }
        public void ResetEqController(EqualizerMode equalizerMode, PlaybackSession playbackSession)
        {
            playbackSession.ToggleEventsOff();

            if (!string.IsNullOrEmpty(playbackSession.CurrentTrackFile))
            {
                PlaybackState _state = playbackSession.PlaybackState;

                string file = playbackSession.CurrentTrackFile;
                playbackSession?.Open(file, playbackSession.audioPlayer.Reader.CurrentTime);

                switch (_state)
                {
                    case PlaybackState.Paused:
                        playbackSession.Pause();
                        break;
                    case PlaybackState.Playing:
                        playbackSession.Play();
                        break;
                    case PlaybackState.Stopped:
                        playbackSession.Stop();
                        break;
                    case PlaybackState.Opened:
                        playbackSession.Open(playbackSession.CurrentTrackFile);
                        break;
                    default:
                        break;
                }
                SetAllBandsGain(0);
            }
            else
            {
                Log.WriteLine("ResetEqController", "reInit failed");
            }

            FireEqUpdated();
            playbackSession.ToggleEventsOn();

        }

        // index: band position
        // double:gain
        public int[] GetBandsGain(EqualizerBand[] equalizerBand)
        {
            if (equalizerBand == null)
            {
                return Array.Empty<int>();
            }
            else
            {
                List<int> GainList = new();
                foreach (var item in equalizerBand)
                {
                    GainList.Add((int)item.Gain);
                }
                return GainList.ToArray();

            }

        }

        internal static EqualizerBand[] Create8Band()
        {
            return new EqualizerBand[]
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
        }

        internal static EqualizerBand[] Create11Band()
        {
            return new EqualizerBand[]
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
        }


        #region Interface
        public void Enable()
        {
            throw new NotImplementedException();
        }

        public void Disable()
        {

            PlaybackSession.ToggleEventsOff();
            PlaybackState _state = PlaybackSession.PlaybackState;
            string file = PlaybackSession.CurrentTrackFile;
            PlaybackSession.Open(file, PlaybackSession.audioPlayer.Reader.CurrentTime);
            switch (_state)
            {
                case PlaybackState.Paused:
                    PlaybackSession.Pause();
                    break;
                case PlaybackState.Playing:
                    PlaybackSession.Play();
                    break;
                case PlaybackState.Stopped:
                    PlaybackSession.Stop();
                    break;
                default:
                    break;
            }
            PlaybackSession.ToggleEventsOn();
        }
        public bool IsEnabled => throw new NotImplementedException();

        public ISampleProvider InputSampleProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ISampleProvider OutputSampleProvider => throw new NotImplementedException();
    }
    #endregion
}