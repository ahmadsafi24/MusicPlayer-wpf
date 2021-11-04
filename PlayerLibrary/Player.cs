using PlayerLibrary.Core;
using PlayerLibrary.Model;
using PlayerLibrary.Shell;
using System;
using static PlayerLibrary.Events;

namespace PlayerLibrary
{
    public class Player
    {
        private readonly NAudioCore nAudioCore;
        public readonly PlaybackSession PlaybackSession;
        public readonly VolumeController VolumeController;
        public readonly EqualizerController EqualizerController;

        public Player(NAudioCore nAudioCore)
        {
            this.nAudioCore = nAudioCore;
        }
        public Player()
        {
            this.nAudioCore = new();
            PlaybackSession = new(nAudioCore);
            EqualizerController = new(PlaybackSession);
            VolumeController = new(PlaybackSession);
        }

    }
}
