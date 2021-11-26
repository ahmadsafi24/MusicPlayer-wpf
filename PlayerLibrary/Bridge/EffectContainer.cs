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
        private readonly NAudioPlayer AudioPlayer;
        internal ISampleProvider source;



        private SmbPitchShiftingSampleProvider PitchController;

        public EqualizerController EqualizerController;

        private ISampleProvider OutSample { get; set; }

        public EffectContainer(NAudioPlayer audioPlayer)
        {
            AudioPlayer = audioPlayer;
            EqualizerController = new(this);
        }
        public WaveFormat WaveFormat => source.WaveFormat;

        public int Read(float[] buffer, int offset, int count)
        {
            int sampleRead = OutSample.Read(buffer, offset, count);
            return sampleRead;
        }


        internal void Init()
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
            OutSample = source;
        }
        private bool enableEqualizer;
        public bool EnableEqualizer
        {
            get => enableEqualizer;
            set
            {
                enableEqualizer = value;
                Init();
                if (EqualizerController != null)
                {
                    EqualizerController.FireEqUpdated();
                }
            }
        }
        internal void ApplyEq()
        {
            if (source == null) return;
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
                Init();
            }
        }

        private float _pitchfactor = 1;
        public float pitchfactor
        {
            get => _pitchfactor;
            set
            {
                _pitchfactor = value;
                if (value == 1)
                {
                    if (EnablePtchShifting == true)
                    {
                        EnablePtchShifting = false;
                    }
                }
                else
                {
                    if (EnablePtchShifting == false)
                    {
                        EnablePtchShifting = true;
                    }
                }
                if (PitchController != null)
                {
                    PitchController.PitchFactor = value;
                }
            }
        }
        private void ApplyPitchShifting()
        {
            if (source == null) return;
            ISampleProvider temp = OutSample;

            PitchController = new(temp) { PitchFactor = pitchfactor };
            OutSample = PitchController;
        }

        public void RaiseOutSampleProviderChanged()
        {
            OutSampleProviderChanged?.Invoke();
        }
        public event EventHandlerEmpty OutSampleProviderChanged;
    }
}
