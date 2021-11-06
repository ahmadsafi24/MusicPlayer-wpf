using PlayerLibrary.Core;
using PlayerLibrary.Shell;

namespace PlayerLibrary
{
    public class SoundPlayer
    {
        private readonly NAudioCore nAudioCore;
        public readonly PlaybackSession PlaybackSession;
        public readonly EqualizerController EqualizerController;

        public SoundPlayer(NAudioCore nAudioCore)
        {
            this.nAudioCore = nAudioCore;
        }
        public SoundPlayer()
        {
            this.nAudioCore = new();
            PlaybackSession = new(nAudioCore);
            EqualizerController = new(PlaybackSession);
        }

    }
}
