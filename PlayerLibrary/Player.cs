using Helper;
using PlayerLibrary.Core;
using PlayerLibrary.Core.NAudioPlayer;
using PlayerLibrary.Core.NAudioPlayer.Interface;
using PlayerLibrary.Plugin;
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
            EqualizerController.Disable();
            EqualizerController = null;
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
