using PlayerLibrary;
using PlayerLibrary.Shell;
using PlayerLibrary.Core;
using PlayerLibrary.Model;
using PlayerLibrary.FileInfo;
using System;
using static PlayerLibrary.Events;
using Helper;

namespace PlayerLibrary.Shell
{
    public class VolumeController
    {
        private readonly NAudioCore nAudioCore;
        private readonly PlaybackSession playbackSession;

        public VolumeController(NAudioCore nAudioCore)
        {
            this.nAudioCore = nAudioCore;
        }

        public VolumeController(PlaybackSession playbackSession)
        {
            this.playbackSession = playbackSession;
            this.nAudioCore = playbackSession.nAudioCore;

        }
        #region Volume
        public event EventHandlerVolume VolumeChanged;
        internal void InvokeVolumeChanged(int newVolume)
        {
            VolumeChanged?.Invoke(newVolume);
        }

        public void VolumeUp(int value)
        {
            if (value > 0)
            {
                ChangeVolume(Volume += value);
            }
        }
        public void VolumeDown(int value)
        {
            if (value > 0)
            {
                ChangeVolume(Volume -= value);
            }
        }
        public void ChangeVolume(int newVolume)
        {
            try
            {
                if (newVolume is >= 0 and <= 100)
                {
                    Volume = newVolume;
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message);
                throw;
            }
        }

        #endregion

        public int Volume
        {
            get
            {
                return (int)(ToDouble(nAudioCore.WaveOutEvent.Volume) * 100);
            }
            set
            {
                try
                {
                    if (IsMuted)
                    {
                        ismute = false;
                    }
                    int iv = value < 0 ? 0 : value > 100 ? 100 : value;
                    double V = (double)iv / 100;
                    //V = V < 0 ? 0 : V > 1 ? 1 : V;
                    nAudioCore.WaveOutEvent.Volume = (float)V;

                    if (iv == 0)
                    {
                        ismute = true;
                    }
                    InvokeVolumeChanged(iv);
                    Log.WriteLine("volume: " + iv);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(ex.Message);
                    throw;
                }
            }
        }
        internal static float ToSingle(double value)
        {
            return (float)value;
        }
        internal static double ToDouble(float value)
        {
            return value;
        }

        private int volBeforeMute;
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