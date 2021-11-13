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
        internal void InvokeVolumeChanged(float newVolume)
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
                    Log.WriteLine("ChangingVolume to", newVolume);
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
                if (NAudioPlayer.VolumeSampleProvider == null)
                {
                    return 1;
                }
                return NAudioPlayer.VolumeSampleProvider.Volume;
            }
            set
            {
                try
                {
                    if (NAudioPlayer.VolumeSampleProvider == null) return;
                    if (value == NAudioPlayer.VolumeSampleProvider.Volume) return;

                    if (IsMuted)
                    {
                        ismute = false;
                    }
                    switch (value)
                    {
                        case >= 0 and <= 1:
                            NAudioPlayer.VolumeSampleProvider.Volume = value;
                            break;
                        default:
                            switch (value)
                            {
                                case > 1:
                                    if (NAudioPlayer.VolumeSampleProvider.Volume == 1) return;
                                    NAudioPlayer.VolumeSampleProvider.Volume = 1;
                                    break;
                                case < 0:
                                    if (NAudioPlayer.VolumeSampleProvider.Volume == 0) return;
                                    NAudioPlayer.VolumeSampleProvider.Volume = 0;
                                    break;
                            }
                            break;
                    }

                    if (value == 0)
                    {
                        ismute = true;
                    }
                    InvokeVolumeChanged(value);
                    Log.WriteLine("volume: " + NAudioPlayer.VolumeSampleProvider.Volume);
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