using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using PlayerLibrary.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlayerLibrary.Events;

namespace PlayerLibrary.Bridge
{
    /// <summary>
    /// Changin Sample Provider Without need to ReInit OutputDevice.
    /// </summary>
    public class EffectContainer : ISampleProvider
    {
        internal ISampleProvider Source { get; set; }



        public EqualizerController EqualizerController;

        private ISampleProvider OutSample { get; set; }

        public EffectContainer()
        {
            EqualizerController = new(this);
        }
        public WaveFormat WaveFormat => Source.WaveFormat;

        public int Read(float[] buffer, int offset, int count)
        {
            int sampleRead = OutSample.Read(buffer, offset, count);
            return sampleRead;
        }


        internal void InitEffects()
        {
            ApplyNoEffect();
            if (EnableEqualizer)
            {
                ApplyEq();

            }
            if (EnablePtchShifting)
            {
                ApplyPitchShifting();
            }


            RaiseOutSampleProviderChanged();
        }
        //_________________________________________________
        public void ApplyNoEffect()
        {
            OutSample = Source;
        }
        private bool enableEqualizer;
        public bool EnableEqualizer
        {
            get => enableEqualizer;
            set
            {
                enableEqualizer = value;
                InitEffects();
                if (EqualizerController != null)
                {
                    EqualizerController.FireEqUpdated();
                }
            }
        }
        internal void ApplyEq()
        {
            if (Source == null)
            {
                return;
            }

            ISampleProvider temp = OutSample;

            EqualizerController.Init(temp);
            OutSample = EqualizerController.OutSampleProvider;
        }

        private bool enablePtchShifting = true;
        public bool EnablePtchShifting
        {
            get => enablePtchShifting;
            set
            {
                enablePtchShifting = value;
                InitEffects();
            }
        }

        public PitchShiftingController PitchShiftingController=new();
        private void ApplyPitchShifting()
        {
            if (Source == null)
            {
                return;
            }

            ISampleProvider temp = OutSample;

            PitchShiftingController.Init(temp);
            OutSample = PitchShiftingController;
        }

        public void RaiseOutSampleProviderChanged()
        {
            OutSampleProviderChanged?.Invoke();
        }
        public event EventHandlerEmpty OutSampleProviderChanged;
    }

    public class PitchShiftingController : ISampleProvider
    {
        private SmbPitchShiftingSampleProvider PitchProvider;
        public void Init(ISampleProvider source)
        {
            PitchProvider = new(source);
        }

        int ISampleProvider.Read(float[] buffer, int offset, int count)
        {
            return PitchProvider.Read(buffer, offset, count);
        }

        private float _pitchfactor = 1;
        public float PitchFactor
        {
            get => _pitchfactor;
            set
            {
                _pitchfactor = value;
                if (PitchProvider != null)
                {
                    PitchProvider.PitchFactor = value;
                }
            }
        }

        WaveFormat ISampleProvider.WaveFormat => PitchProvider.WaveFormat;

    }
}
