using Helper;
using NAudio.Extras;
using NAudio.Wave;
using PlayerLibrary.Core;
using PlayerLibrary.Model;
using System;
using System.Collections.Generic;
using static PlayerLibrary.Events;

namespace PlayerLibrary.Bridge
{
    public class EqualizerController
    {
        #region fields
        private readonly EffectContainer EffectContainer;
        public EqualizerController(EffectContainer EffectContainer)
        {
            this.EffectContainer = EffectContainer;
        }
        #endregion
        EqualizerBand[] bands = Create8Band();
        Equalizer EqualizerCore;
        #region property
        private EqualizerMode _equalizerMode;
        public EqualizerMode EqualizerMode
        {
            get => _equalizerMode;
            set
            {
                _equalizerMode = value;
                SetAllBandsGain(0);
                if (value == EqualizerMode.Normal)
                {
                    List<EqualizerBand> temp = new(Create8Band());
                    bands = temp.ToArray();
                }
                else
                {
                    List<EqualizerBand> temp = new(Create11Band());
                    bands = temp.ToArray();
                }
                if (IsEnabled)
                {
                    SetAllBandsGain(0);
                    EffectContainer.EnableEqualizer=true;
                }
                EqualizerCore?.Update();
                FireEqUpdated();
            }
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
            }
            SetAllBands(preset.BandsGain);

        }

        public EqPreset GetEqPreset()
        {
            EqPreset preset = new(EqualizerMode.ToString(), AllBandsGain);
            return preset;
        }

        #region Event
        public event EventHandlerEmpty EqUpdated;

        public EqualizerController()
        {
        }


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

        public bool IsEnabled { get; private set; }
        private ISampleProvider InputSP;
        internal void Init(ISampleProvider InputSP)
        {
            this.InputSP = InputSP;
            EqualizerCore = new(InputSP, bands);
            IsEnabled = true;
        }
        internal ISampleProvider OutSampleProvider => EqualizerCore;
    }
    #endregion
}