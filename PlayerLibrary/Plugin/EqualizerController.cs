using Helper;
using NAudio.Extras;
using PlayerLibrary.Core;
using PlayerLibrary.Core.NAudioPlayer;
using PlayerLibrary.Model;
using System;
using System.Collections.Generic;
using static PlayerLibrary.Events;

namespace PlayerLibrary.Plugin
{
    public class EqualizerController : IPlugin
    {
        #region fields
        private NAudioPlayerEq nAudioPlayerEq = new();
        private PlaybackSession PlaybackSession;

        #endregion

        #region property
        private EqualizerMode _equalizerMode;
        public EqualizerMode EqualizerMode
        {
            get
            {
                return _equalizerMode;
            }
            set
            {
                _equalizerMode = value;
            }

        }

        /// <summary>
        /// get all bands gain in single array: array length = bands count ,int array[index] = int Allbands[index]
        /// </summary>
        /// <value></value>
        public int[] AllBandsGain { get => GetBandsGain(nAudioPlayerEq.EqualizerBand); }

        #endregion
        public EqualizerController(PlaybackSession playbackSession, EqualizerMode equalizerMode)
        {
            this.PlaybackSession = playbackSession;
            EqualizerMode = equalizerMode;
            ResetEqController(equalizerMode, playbackSession, nAudioPlayerEq);
            FireEqUpdated();
        }

        public void SetEqPreset(EqPreset preset)
        {
            EqualizerMode mode = (EqualizerMode)Enum.Parse(typeof(PlayerLibrary.EqualizerMode), preset.EqualizerMode);
            if (mode != EqualizerMode)
            {
                EqualizerMode = mode;
                ResetEqController(mode, PlaybackSession, nAudioPlayerEq);
            }
            SetAllBands(preset.BandsGain);
        }

        public EqPreset GetEqPreset()
        {
            EqPreset preset = new(EqualizerMode.ToString(), AllBandsGain);
            return preset;
        }

        #region Event
        public EventHandlerEmpty EqUpdated;
        internal void FireEqUpdated()
        {
            EqUpdated?.Invoke();
        }

        #endregion

        /// <summary>
        /// Change a single band gain 
        /// </summary>
        /// <param name="bandIndex"> band index</param>
        /// <param name="gain"> band gain</param>
        internal void SetBandGain(int bandIndex, float gain)
        {
            if (bandIndex > nAudioPlayerEq.EqualizerBand.Length)
            {
                Helper.Log.WriteLine("band index bigger then equalizerband length");
                return;
            }
            else
            {
                Log.WriteLine("changing band index:" + bandIndex.ToString());
                nAudioPlayerEq.EqualizerBand[bandIndex].Gain = gain;
                nAudioPlayerEq.EqualizerCore?.Update();
            }
        }

        public double GetBandGain(int bandindex)
        {
            if (nAudioPlayerEq.EqualizerBand == null)
            {
                return 0;
            }
            if (nAudioPlayerEq.EqualizerBand.Length <= bandindex)
            {
                return 0;
            }
            else
            {
                return nAudioPlayerEq.EqualizerBand[bandindex].Gain;
            }
        }

        internal void SetBand(int bandIndex, float Gain, float Bandwidth, float Frequency)
        {
            if (nAudioPlayerEq.EqualizerBand == null) return;
            if (Gain != 0) { nAudioPlayerEq.EqualizerBand[bandIndex].Gain = Gain; }
            if (Bandwidth != 0) { nAudioPlayerEq.EqualizerBand[bandIndex].Bandwidth = Bandwidth; }
            if (Frequency != 0) { nAudioPlayerEq.EqualizerBand[bandIndex].Frequency = Frequency; }
            nAudioPlayerEq.EqualizerCore?.Update();
        }

        public void SetAllBandsGain(int gain)
        {
            foreach (var item in nAudioPlayerEq.EqualizerBand)
            {
                item.Gain = 0;
            }
            nAudioPlayerEq.EqualizerCore?.Update();
            FireEqUpdated();
        }

        public void SetBandGain(int bandIndex, float Gain, bool requestNotifyEqUpdate)
        {
            try
            {
                SetBandGain(bandIndex, Gain);
                if (requestNotifyEqUpdate)
                {
                    FireEqUpdated();
                }
            }
            catch (System.Exception ex)
            {
                Helper.Log.WriteLine("SetBandGain", ex.Message);
                throw;
            }
        }

        public void SetAllBands(int[] Bands)
        {
            int i = 0;
            foreach (var item in Bands)
            {
                SetBandGain(i, (float)item);
                FireEqUpdated();
                i++;
            }
        }

        public void RequestResetEqController() { ResetEqController(EqualizerMode, PlaybackSession, nAudioPlayerEq); }
        public void ResetEqController(EqualizerMode equalizerMode, PlaybackSession playbackSession, NAudioPlayerEq core)
        {
            playbackSession.ToggleEventsOff();

            InstalEqualizer(equalizerMode, nAudioPlayerEq);

            if (!string.IsNullOrEmpty(playbackSession.CurrentTrackFile))
            {
                PlaybackState _state = playbackSession.PlaybackState;

                float lastvolume = 0;
                if (playbackSession.NAudioPlayer.VolumeSampleProvider != null)
                {
                    lastvolume = playbackSession.NAudioPlayer.VolumeSampleProvider.Volume;
                }

                string file = playbackSession.CurrentTrackFile;
                playbackSession?.Open(file, nAudioPlayerEq.Reader.CurrentTime);
                switch (_state)
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
                    case PlaybackState.Opened:
                        playbackSession.Open(playbackSession.CurrentTrackFile);
                        break;
                    default:
                        break;
                }
                SetAllBandsGain(0);
                playbackSession.NAudioPlayer.VolumeSampleProvider.Volume = lastvolume;
            }
            else
            {
                Log.WriteLine("ResetEqController", "reInit failed");
            }

            FireEqUpdated();
            playbackSession.ToggleEventsOn();

        }

        private void InstalEqualizer(EqualizerMode equalizerMode, NAudioPlayerEq _nAudioPlayer)
        {
            if (PlaybackSession.NAudioPlayerType != typeof(NAudioPlayerEq))
            {
                _nAudioPlayer.Reader = PlaybackSession.NAudioPlayer.Reader;
                _nAudioPlayer.VolumeSampleProvider = PlaybackSession.NAudioPlayer.VolumeSampleProvider;
                _nAudioPlayer.OutputDevice = PlaybackSession.NAudioPlayer.OutputDevice;
                PlaybackSession.NAudioPlayer = _nAudioPlayer;
            }
            Log.WriteLine($"creating equalizer bands for: {equalizerMode}");
            switch (equalizerMode)
            {
                case EqualizerMode.Normal:
                    Create8Band(_nAudioPlayer);
                    break;
                case EqualizerMode.Super:
                    Create11Band(_nAudioPlayer);
                    break;
                default:
                    break;
            }
        }

        // index: band position
        // double:gain
        public int[] GetBandsGain(EqualizerBand[] equalizerBand)
        {
            if (equalizerBand == null)
            {
                return Array.Empty<int>();
            }
            else
            {
                List<int> GainList = new();
                foreach (var item in equalizerBand)
                {
                    GainList.Add((int)item.Gain);
                }
                return GainList.ToArray();

            }

        }

        internal static void Create8Band(NAudioPlayerEq nAudioCore)
        {
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
        }

        internal static void Create11Band(NAudioPlayerEq nAudioCore)
        {
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
        }


        #region Interface
        public void Enable()
        {
            throw new NotImplementedException();
        }

        public void Disable()
        {
            //this = null;
            NAudioPlayerNormal _nAudioPlayer = new NAudioPlayerNormal();

            _nAudioPlayer.Reader = PlaybackSession.NAudioPlayer.Reader;
            _nAudioPlayer.VolumeSampleProvider = PlaybackSession.NAudioPlayer.VolumeSampleProvider;
            _nAudioPlayer.OutputDevice = PlaybackSession.NAudioPlayer.OutputDevice;
            PlaybackSession.NAudioPlayer = _nAudioPlayer;

            PlaybackSession.ToggleEventsOff();
            PlaybackState _state = PlaybackSession.PlaybackState;
            float lastvolume = PlaybackSession.NAudioPlayer.VolumeSampleProvider.Volume;
            string file = PlaybackSession.CurrentTrackFile;
            PlaybackSession.Open(file, _nAudioPlayer.Reader.CurrentTime);
            switch (_state)
            {
                case PlaybackState.Paused:
                    PlaybackSession.Pause();
                    break;
                case PlaybackState.Playing:
                    PlaybackSession.Play();
                    break;
                case PlaybackState.Stopped:
                    PlaybackSession.Stop();
                    break;
                default:
                    break;
            }
            PlaybackSession.NAudioPlayer.VolumeSampleProvider.Volume = lastvolume;
            PlaybackSession.RaiseNAudioPlayerChanged(_nAudioPlayer.GetType());
        }
        public bool IsEnabled => throw new NotImplementedException();
    }
    #endregion
}