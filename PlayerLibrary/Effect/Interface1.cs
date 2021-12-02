using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerLibrary.Effect
{
    internal interface IEffect : ISampleProvider, IDisposable
    {
        bool Enable { get; set; }
        ISampleProvider Input { get; set; }// Use when really needed / first try to use IsamplProvider.read
        ISampleProvider Output { get; set; }// Use when really needed / first try to use IsamplProvider.read
    }

    public class VolumeEffect : IEffect
    {
        #region IEffect
        bool IEffect.Enable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        ISampleProvider IEffect.Input { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        ISampleProvider IEffect.Output { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion

        #region ISampleProvider
        WaveFormat ISampleProvider.WaveFormat => throw new NotImplementedException();
        int ISampleProvider.Read(float[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~VolumeEffect()
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

        #endregion
    }

    public class EffectImplementSampleClass : IEffect
    {
        #region IEffect
        bool IEffect.Enable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        ISampleProvider IEffect.Input { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        ISampleProvider IEffect.Output { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion

        #region ISampleProvider
        WaveFormat ISampleProvider.WaveFormat => throw new NotImplementedException();
        int ISampleProvider.Read(float[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~VolumeEffect()
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

        #endregion
    }
}
