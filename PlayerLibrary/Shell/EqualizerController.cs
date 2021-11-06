using NAudio.Extras;
using System;
using PlayerLibrary.Core;
using PlayerLibrary.Model;
using System.Collections.Generic;
using Helper;
using static PlayerLibrary.Events;

namespace PlayerLibrary.Shell
{
    public class EqualizerController
    {
        #region fields
        private readonly NAudioCore nAudioCore;
        private readonly PlaybackSession playbackSession;

        #endregion

        #region property
        private EqualizerMode _equalizerMode;
        public EqualizerMode EqualizerMode
        {
            get
            {
                return _equalizerMode;
            }
            set
            {
                _equalizerMode = value;
            }

        }

        /// <summary>
        /// get all bands gain in single array: array length = bands count ,int array[index] = int Allbands[index]
        /// </summary>
        /// <value></value>
        public int[] AllBandsGain { get => GetBandsGain(nAudioCore.EqualizerBand); }
        #endregion

        public EqualizerController(PlaybackSession playbackSession)
        {
            this.playbackSession = playbackSession;
            this.nAudioCore = playbackSession.nAudioCore;
            EqualizerMode = EqualizerMode.Super;
            ResetEqController();
        }

        public void SetEqPreset(EqPreset preset)
        {
            EqualizerMode = (EqualizerMode)Enum.Parse(typeof(PlayerLibrary.EqualizerMode), preset.EqualizerMode);
            ResetEqController();
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
            if (bandIndex > nAudioCore.EqualizerBand.Length)
            {
                Helper.Log.WriteLine("band index bigger then equalizerband length");
                return;
            }
            else
            {
                Log.WriteLine("changing band index:" + bandIndex.ToString());
                nAudioCore.EqualizerBand[bandIndex].Gain = gain;
                nAudioCore.EqualizerCore?.Update();
            }
        }

        public double GetBandGain(int bandindex)
        {
            if (nAudioCore.EqualizerBand.Length <= bandindex)
            {
                return 0;
            }
            else
            {
                return nAudioCore.EqualizerBand[bandindex].Gain;
            }
        }

        internal void SetBand(int bandIndex, float Gain, float Bandwidth, float Frequency)
        {
            if (nAudioCore.EqualizerBand == null) return;
            if (Gain != 0) { nAudioCore.EqualizerBand[bandIndex].Gain = Gain; }
            if (Bandwidth != 0) { nAudioCore.EqualizerBand[bandIndex].Bandwidth = Bandwidth; }
            if (Frequency != 0) { nAudioCore.EqualizerBand[bandIndex].Frequency = Frequency; }
            nAudioCore.EqualizerCore?.Update();
        }

        public void SetAllBandsGain(int gain)
        {
            foreach (var item in nAudioCore.EqualizerBand)
            {
                item.Gain = 0;
            }
            nAudioCore.EqualizerCore?.Update();
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
                Helper.Log.WriteLine(ex.Message);
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

        public void ResetEqController()
        {
            playbackSession.ToggleEventsOff();
            ///playbackSession.Stop();
            //if (nAudioCore.EqualizerBand == null) return;

            InstalEqualizer(this.EqualizerMode, this.nAudioCore, this.playbackSession);
            SetAllBandsGain(0);
            FireEqUpdated();

            if (!string.IsNullOrEmpty(playbackSession.AudioFilePath))
            {
                PlaybackState _state = playbackSession.PlaybackState;
                playbackSession.Open(playbackSession.AudioFilePath, nAudioCore.Reader.CurrentTime);
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
                    default:
                        break;
                }
            }

            playbackSession.ToggleEventsOn();

        }

        private static void InstalEqualizer(EqualizerMode equalizerMode, NAudioCore nAudioCore, PlaybackSession playbackSession)
        {
            switch (equalizerMode)
            {
                case EqualizerMode.Normal:
                    Create8Band(nAudioCore);
                    playbackSession.IsEqEnabled = true;
                    break;
                case EqualizerMode.Super:
                    Create11Band(nAudioCore);
                    playbackSession.IsEqEnabled = true;
                    break;
                case EqualizerMode.Disabled:
                    CreateEmptyBand(nAudioCore);
                    playbackSession.IsEqEnabled = false;
                    break;
                default:
                    break;
            }
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

        internal static void Create8Band(NAudioCore nAudioCore)
        {
            nAudioCore.EqualizerBand = new EqualizerBand[]
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

        internal static void Create11Band(NAudioCore nAudioCore)
        {
            nAudioCore.EqualizerBand = new EqualizerBand[]
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
        internal static void CreateEmptyBand(NAudioCore core)
        {
            core.EqualizerBand = Array.Empty<EqualizerBand>();
        }
    }
}