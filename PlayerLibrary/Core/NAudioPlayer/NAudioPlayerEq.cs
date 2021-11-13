using Helper;
using NAudio.CoreAudioApi;
using NAudio.Extras;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using PlayerLibrary.Core.NAudioPlayer.Interface;
using System;
using System.Collections.Generic;

namespace PlayerLibrary.Core.NAudioPlayer
{
    //TODO: EngineMode  like with eq or with mono or pitch changable
    public class NAudioPlayerEq : INAudioPlayer
    {
        #region NAudio Engine
        internal WaveStream Reader;//= new MediaFoundationReader(file);

        internal EqualizerBand[] EqualizerBand { get; set; } // to control equalizer bands

        internal Equalizer EqualizerCore;// to init equalizer
        private bool disposedValue;

        internal VolumeSampleProvider VolumeSampleProvider { get; set; }

        internal IWavePlayer OutputDevice { get; set; } = new WaveOutEvent(); // player engine WaveOutEvent | WasapiOut | DirectSoundOut

        public void Init()
        {
            Log.WriteLine("Init: with eq");

            if (VolumeSampleProvider != null)
            {
                float vol = VolumeSampleProvider.Volume;
                VolumeSampleProvider = new(Reader.ToSampleProvider());
                VolumeSampleProvider.Volume = vol;
            }
            else
            {
                VolumeSampleProvider = new(Reader.ToSampleProvider());
            }
            VolumeSampleProvider = new(Reader.ToSampleProvider());
            EqualizerCore = new(VolumeSampleProvider, EqualizerBand);
            OutputDevice.Init(EqualizerCore);
        }
        #endregion

        WaveStream INAudioPlayer.Reader { get => Reader; set => Reader = value; }

        VolumeSampleProvider INAudioPlayer.VolumeSampleProvider { get => VolumeSampleProvider; set => VolumeSampleProvider = value; }

        IWavePlayer INAudioPlayer.OutputDevice { get => OutputDevice; set => OutputDevice = value; }

        void INAudioPlayer.Init()
        {
            Init();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    OutputDevice.Dispose();
                    EqualizerCore = null;
                    EqualizerBand = null;
                    VolumeSampleProvider = null;
                    Reader.Dispose();

                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~NAudioPlayer()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
