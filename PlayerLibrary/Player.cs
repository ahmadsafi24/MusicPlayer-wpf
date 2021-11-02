using PlayerLibrary.Core;
using PlayerLibrary.Model;
using System;
using static PlayerLibrary.Events;

namespace PlayerLibrary
{
    public class Player
    {
        public readonly Shell.Timing Timing;
        public readonly Shell.Controller Controller;

        #region base
        internal readonly NAudioCore nAudioCore;

        internal Player(NAudioCore nAudioCore)
        {
            this.nAudioCore = nAudioCore;
        }
        public Player()
        {
            this.nAudioCore = new NAudioCore(this);
            Timing = new(this);
            Controller = new(this);
        }
        #endregion

        #region Events
        public event EventHandlerPlaybackState PlaybackStateChanged;

        internal void InvokePlaybackStateChanged(PlaybackState value)
        {
            PlaybackStateChanged?.Invoke(value);
        }

        #endregion

        #region Volume
        public event EventHandlerVolume VolumeChanged;
        internal void InvokeVolumeChanged(int newVolume)
        {
            VolumeChanged?.Invoke(newVolume);
        }

        public int Volume { get => nAudioCore.Volume; private set => nAudioCore.Volume = value; }
        public bool IsMuted { get => nAudioCore.IsMuted; set => nAudioCore.IsMuted = value; }

        public void VolumeUp(int value)
        {
            if (value > 0)
            {
                ChangeVolume(nAudioCore.Volume += value);
            }
        }
        public void VolumeDown(int value)
        {
            if (value > 0)
            {
                ChangeVolume(nAudioCore.Volume -= value);
            }
        }
        public void ChangeVolume(int newVolume)
        {
            try
            {
                if (newVolume is >= 0 and <= 100)
                {
                    nAudioCore.Volume = newVolume;
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message);
            }
        }

        #endregion

        #region Eq

        public EqualizerMode EqualizerMode { get => nAudioCore.equalizerMode; set => nAudioCore.equalizerMode = value; }

        public void ResetEq() => nAudioCore.ResetEq();

        public void ChangeEq(int bandIndex, float Gain, bool requestNotifyEqUpdate) => nAudioCore.ChangeEq(bandIndex, Gain, requestNotifyEqUpdate);

        public double GetEqBandGain(int BandIndex) => nAudioCore.GetEqBandGain(BandIndex);

        public int[] EqBandsGain { get => nAudioCore.BandsGain; }

        public void ReIntialEq() => nAudioCore.ReIntialEq();

        public void ChangeBands(int[] bands) => nAudioCore.ChangeBands(bands);

        public void ImportEq(EqPreset preset) => ChangeBands(preset.BandsGain);

        public void ExportEq(string filepath)
        {
            EqPreset EqPreset = new(EqualizerMode, EqBandsGain);
            Preset.Equalizer.PresetToFile(EqPreset, filepath);
        }

        public EventHandlerEmpty EqUpdated;
        internal void FireEqUpdated()
        {
            EqUpdated?.Invoke();
        }
        #endregion

    }
}
