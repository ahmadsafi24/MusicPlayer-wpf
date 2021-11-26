using Helper;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using static PlayerLibrary.Events;

namespace PlayerLibrary.Core
{
    public class VolumeController
    {
        public VolumeSampleProvider volumeProvider = new(null);

        public ISampleProvider InputSampleProvider { get; set; }

        public ISampleProvider OutputSampleProvider
        {
            get
            {
                volumeProvider = new(InputSampleProvider) { Volume = InitVol() };
                return volumeProvider;
            }
        }

        private float InitVol()
        {
            if (volumeProvider != null)
            {
                return volumeProvider.Volume;
            }
            else
            {
                return 1;
            }
        }

        #region Volume
        public event EventHandlerVolume VolumeChanged;
        internal void RaiseVolumeChanged(float newVolume)
        {
            VolumeChanged?.Invoke(newVolume);
        }

        public void VolumeUp(float value)
        {
            if (value > 0)
            {
                ChangeVolume(Volume += value);
            }
        }
        public void VolumeDown(float value)
        {
            if (value > 0)
            {
                ChangeVolume(Volume -= value);
            }
        }
        public void ChangeVolume(float newVolume)
        {
            try
            {
                if (newVolume is >= 0 and <= 1)
                {
                    Volume = newVolume;
                    //Log.WriteLine("ChangingVolume to", newVolume);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine("ChangeVolume", ex.Message);
                throw;
            }
        }




        #endregion

        public float Volume
        {
            get
            {
                if (volumeProvider == null)
                {
                    return 1;
                }
                return volumeProvider.Volume;
            }
            set
            {
                try
                {
                    if (volumeProvider == null)
                    {
                        return;
                    }

                    if (value == volumeProvider.Volume)
                    {
                        return;
                    }

                    if (IsMuted)
                    {
                        ismute = false;
                    }
                    switch (value)
                    {
                        case >= 0 and <= 1:
                            volumeProvider.Volume = value;
                            break;
                        default:
                            switch (value)
                            {
                                case > 1:
                                    if (volumeProvider.Volume == 1)
                                    {
                                        return;
                                    }

                                    volumeProvider.Volume = 1;
                                    break;
                                case < 0:
                                    if (volumeProvider.Volume == 0)
                                    {
                                        return;
                                    }

                                    volumeProvider.Volume = 0;
                                    break;
                            }
                            break;
                    }

                    if (value == 0)
                    {
                        ismute = true;
                    }
                    RaiseVolumeChanged(value);
                    Log.WriteLine("volume: " + volumeProvider.Volume);
                }
                catch (Exception ex)
                {
                    Log.WriteLine("Volume", ex.Message);
                    throw;
                }
            }
        }

        private float volBeforeMute;
        private bool ismute;
        public bool IsMuted
        {
            get => ismute;
            set
            {
                if (value)
                {
                    volBeforeMute = Volume;
                    Volume = 0;
                }
                else
                {
                    Volume = volBeforeMute;
                }
                ismute = value;
            }
        }


    }
}