using NAudio.CoreAudioApi;
using NAudio.Extras;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;

namespace PlayerLibrary.Core.NAudioPlayer.Interface
{
    //TODO: EngineMode  like with eq or with mono or pitch changable
    public interface INAudioPlayer : IDisposable
    {
        #region NAudio Engine
        public WaveStream Reader { get; set; }// for timeline control: MediaFoundationReader(file);

        public VolumeSampleProvider VolumeSampleProvider { get; set; }// for volume control

        public IWavePlayer OutputDevice { get; set; } // for play-pause-more.: WaveoutEvent | WasapiOut | DirectSoundOut

        #endregion
        public void Init();

    }
}
