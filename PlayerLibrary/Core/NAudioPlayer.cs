using Helper;
using NAudio.CoreAudioApi;
using NAudio.Extras;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;

namespace PlayerLibrary.Core
{
    //TODO: EngineMode  like with eq or with mono or pitch changable
    public class NAudioPlayer
    {
        #region NAudio Engine
        internal WaveStream Reader { get; set; } //= new // MediaFoundationReader | AudioFileReader | Mp3FileReader | WaveFileReader

        internal ISampleProvider SampleProvider { get; set; }

        internal IWavePlayer OutputDevice { get; set; } // = new // player engine WaveOutEvent | WasapiOut | DirectSoundOut

        public void Init()
        {
            OutputDevice.Dispose();
            Reader.Dispose();

            OutputDevice.Init(SampleProvider);
        }
        #endregion


    }
}
