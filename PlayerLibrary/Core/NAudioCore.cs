using NAudio.Extras;
using NAudio.Wave;
using System;
using System.Collections.Generic;

namespace PlayerLibrary.Core
{
    //TODO: EngineMode  like with eq or with mono or pitch changable
    public class NAudioCore
    {
        #region NAudio Engine
        internal MediaFoundationReader Reader;
        internal ISampleProvider SampleProvider => Reader.ToSampleProvider();
        internal EqualizerBand[] EqualizerBand { get; set; }
        internal Equalizer EqualizerCore;
        internal WaveOutEvent WaveOutEvent = new();
        #endregion

        internal NAudioCore()
        {

        }

    }
}
