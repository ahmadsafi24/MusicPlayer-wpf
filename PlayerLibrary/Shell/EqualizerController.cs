using PlayerLibrary;
using PlayerLibrary.Shell;
using PlayerLibrary.Core;
using PlayerLibrary.Model;
using PlayerLibrary.FileInfo;
using static PlayerLibrary.Events;
using System.Collections.Generic;
using NAudio.Extras;
using System;

namespace PlayerLibrary.Shell
{
    public class EqualizerController
    {
        private readonly NAudioCore nAudioCore;
        private readonly PlaybackSession playbackSession;

        internal EqualizerController(PlaybackSession playbackSession)
        {
            this.playbackSession = playbackSession;
            this.nAudioCore = playbackSession.nAudioCore;
            InitialEqualizer();
        }


        #region Eq

        public EqualizerMode EqualizerMode { get; set; } = EqualizerMode.Super;

        public int[] EqBandsGain { get => BandsGain; }

        public void ImportEq(EqPreset preset) => ChangeAllBands(preset.BandsGain);

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




        internal void ChangeEqualizerBand(int bandIndex, float gain)
        {
            nAudioCore.EqualizerBand[bandIndex].Gain = gain;
            nAudioCore.EqualizerCore?.Update();
        }

        public double GetEqBandGain(int bandindex)
        {
            return nAudioCore.EqualizerBand[bandindex].Gain;
        }

        internal void ChangeEqualizerBand(int bandIndex, float Gain, float Bandwidth, float Frequency)
        {
            if (Gain != 0) { nAudioCore.EqualizerBand[bandIndex].Gain = Gain; }
            if (Bandwidth != 0) { nAudioCore.EqualizerBand[bandIndex].Bandwidth = Bandwidth; }
            if (Frequency != 0) { nAudioCore.EqualizerBand[bandIndex].Frequency = Frequency; }
            nAudioCore.EqualizerCore?.Update();
        }

        public void ResetEq()
        {
            foreach (var item in nAudioCore.EqualizerBand)
            {
                item.Gain = 0;
            }
            nAudioCore.EqualizerCore?.Update();
        }

        // index: band position
        // double:gain
        internal int[] BandsGain
        {
            get
            {
                List<int> GainList = new();
                foreach (var item in nAudioCore.EqualizerBand)
                {
                    GainList.Add((int)item.Gain);
                }
                return GainList.ToArray();
            }
        }

        public void ChangeEqBandGain(int bandIndex, float Gain, bool requestNotifyEqUpdate)
        {
            ChangeEqualizerBand(bandIndex, Gain);
            if (requestNotifyEqUpdate)
            {
                FireEqUpdated();
            }
        }

        public void ChangeAllBands(int[] Bands)
        {
            EqualizerMode newEqMode = new();
            if (Bands.Length == 8)
            {
                newEqMode = EqualizerMode.Normal;
            }
            else if (Bands.Length == 12)
            {
                newEqMode = EqualizerMode.Super;
            }
            if (EqualizerMode != newEqMode)
            {
                EqualizerMode = newEqMode;
                ReIntialEq();
            }
            int i = 0;
            foreach (var item in Bands)
            {
                ChangeEqualizerBand(i, (float)item);
                FireEqUpdated();
                i++;
            }
        }

        internal void InitialEqualizer()
        {
            switch (EqualizerMode)
            {
                case EqualizerMode.Normal:
                    nAudioCore.EqualizerBand = new EqualizerBand[]
                     {
                        new EqualizerBand { Bandwidth = 0.8f, Frequency = 100, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.8f, Frequency = 200, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.8f, Frequency = 400, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.8f, Frequency = 800, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.8f, Frequency = 1200, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.8f, Frequency = 2400, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.8f, Frequency = 4800, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.8f, Frequency = 9600, Gain = 0 },
                     };
                    playbackSession.IsEqEnabled = true;
                    break;
                case EqualizerMode.Super:
                    nAudioCore.EqualizerBand = new EqualizerBand[]
                    {
                        new EqualizerBand { Bandwidth = 0.4f, Frequency = 30, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.4f, Frequency = 50, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.4f, Frequency = 90, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.4f, Frequency = 160, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.4f, Frequency = 300, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.4f, Frequency = 500, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.4f, Frequency = 1000, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.4f, Frequency = 1600, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.4f, Frequency = 3000, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.4f, Frequency = 5000, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.4f, Frequency = 9000, Gain = 0 },
                        new EqualizerBand { Bandwidth = 0.4f, Frequency = 16000, Gain = 0 },
                    };
                    playbackSession.IsEqEnabled = true;
                    break;
                case EqualizerMode.Disabled:
                    nAudioCore.EqualizerBand = Array.Empty<EqualizerBand>();
                    playbackSession.IsEqEnabled = false;
                    break;
                default:
                    break;
            }
        }


        public void ReIntialEq()
        {
            PlaybackState lastpstate = playbackSession.PlaybackState;
            playbackSession.ToggleEventsOff();

            InitialEqualizer();
            if (!string.IsNullOrEmpty(playbackSession.AudioFilePath))
            {
                playbackSession.Open(playbackSession.AudioFilePath, nAudioCore.Reader.CurrentTime);
                switch (lastpstate)
                {
                    case PlaybackState.Paused:
                        playbackSession.Pause();
                        break;
                    case PlaybackState.Playing:
                        playbackSession.Play();
                        break;
                    case PlaybackState.Stopped:
                        playbackSession.Stop();
                        break;
                    default:
                        break;
                }
            }

            playbackSession.ToggleEventsOn();
            FireEqUpdated();
        }

    }
}