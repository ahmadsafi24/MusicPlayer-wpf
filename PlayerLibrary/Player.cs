using Helper;
using PlayerLibrary.Core;
using PlayerLibrary.Core.NAudioPlayer;
using PlayerLibrary.Core.NAudioPlayer.Interface;
using System;
using System.Text;
using static PlayerLibrary.Events;

namespace PlayerLibrary
{
    public class Player
    {
        public readonly PlaybackSession PlaybackSession;
        public EqualizerController EqualizerController;

        public Player(INAudioPlayer nAudioPlayer)
        {
            Log.WriteLine("new player with type: " + nAudioPlayer.GetType());
            PlaybackSession = new(nAudioPlayer);
        }
        public Player()
        {
            INAudioPlayer nAudioPlayer = new NAudioPlayerNormal();
            PlaybackSession = new(nAudioPlayer);
        }

        public void EnableEqualizerController()
        {
            EqualizerController = new(PlaybackSession, EqualizerMode.Super);
            PropertyChanged?.Invoke();
        }

        public void DisableEqualizerController()
        {
            EqualizerController = null;
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


        public EventHandlerEmpty PropertyChanged;

        public string GetInfo()
        {
            StringBuilder stringBuilder = new();

            stringBuilder.AppendLine($"PlaybackSession: < {PlaybackSession} >");
            stringBuilder.AppendLine($"Audiofilepath: < {PlaybackSession?.CurrentTrackFile} >");
            stringBuilder.AppendLine($"TimelineCurrent: < {PlaybackSession.TimelineController.Current.ToString()} >");

            return stringBuilder.ToString();
        }
    }
}
