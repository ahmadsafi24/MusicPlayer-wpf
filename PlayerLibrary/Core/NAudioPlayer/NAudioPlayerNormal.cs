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
    public class NAudioPlayerNormal : INAudioPlayer
    {
        #region NAudio Engine
        internal WaveStream Reader;//= new MediaFoundationReader(file);
        private bool disposedValue;

        internal VolumeSampleProvider VolumeSampleProvider { get; set; }

        internal IWavePlayer OutputDevice { get; set; } = new WaveOutEvent(); // player engine WaveOutEvent | WasapiOut | DirectSoundOut

        public void Init()
        {
            Log.WriteLine("Init: normal");
            VolumeSampleProvider = new(Reader.ToSampleProvider());
            OutputDevice.Init(VolumeSampleProvider);
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
                    VolumeSampleProvider = null;
                    Reader?.Dispose();
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
