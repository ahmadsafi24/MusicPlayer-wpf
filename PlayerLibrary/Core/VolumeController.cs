using PlayerLibrary;
using PlayerLibrary.Core;
using PlayerLibrary.Model;
using PlayerLibrary.FileInfo;
using System;
using Helper;
using PlayerLibrary.Core.NAudioPlayer.Interface;
using PlayerLibrary.Core.NAudioPlayer;
using static PlayerLibrary.Events;

namespace PlayerLibrary.Core
{
    public class VolumeController
    {
        internal INAudioPlayer NAudioPlayer { get; set; }

        public VolumeController(NAudioPlayerEq nAudioCore)
        {
            this.NAudioPlayer = nAudioCore;
        }
        public VolumeController(PlaybackSession playbackSession)
        {
            Log.WriteLine("new VolumeController");
            this.NAudioPlayer = playbackSession.NAudioPlayer;
            playbackSession.PlaybackStateChanged += PlaybackStateChanged;
            InvokeVolumeChanged(Volume);

        }
        private void PlaybackStateChanged(PlaybackState state)
        {
            if (state == PlaybackState.Opened)
            {
                InvokeVolumeChanged(Volume);
            }
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
                Log.WriteLine("ChangeVolume", ex.Message);
                throw;
            }
        }

        #endregion

        public int Volume
        {
            get
            {
                if (NAudioPlayer.VolumeSampleProvider == null)
                {
                    return 0;
                }
                return (int)(ToDouble(NAudioPlayer.VolumeSampleProvider.Volume) * 100);
            }
            set
            {
                try
                {
                    if (NAudioPlayer.VolumeSampleProvider == null) return;

                    if (IsMuted)
                    {
                        ismute = false;
                    }
                    int iv = value < 0 ? 0 : value > 100 ? 100 : value;
                    double V = (double)iv / 100;
                    //V = V < 0 ? 0 : V > 1 ? 1 : V;
                    NAudioPlayer.VolumeSampleProvider.Volume = (float)V;

                    if (iv == 0)
                    {
                        ismute = true;
                    }
                    InvokeVolumeChanged(iv);
                    Log.WriteLine("volume: " + iv);
                }
                catch (Exception ex)
                {
                    Log.WriteLine("Volume", ex.Message);
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