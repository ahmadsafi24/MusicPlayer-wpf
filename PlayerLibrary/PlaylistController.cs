using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlayerLibrary.Core;

namespace PlayerLibrary
{
    public class PlaylistController
    {
        public Dictionary<int, string> playlist;
        private PlaybackSession playbackSession;

        public PlaylistController(PlaybackSession _playbackSession)
        {
            playlist = new();
            playbackSession = _playbackSession;
        }

        private void PlaybackStateChanged(PlaybackState state)
        {
            switch (state)
            {
                case PlaybackState.Ended:
                    OnPlaybackEnded();
                    break;
                default:
                    break;
            }

        }
        public async Task NextTrackAsync(PlaybackSession _playbackSession)
        {
            await playbackSession.OpenAsync(new Uri(playlist[0]));
        }

        public async Task PreviosTrackAsync(PlaybackSession _playbackSession)
        {
            await playbackSession.OpenAsync(new Uri(playlist[0]));
        }

        public void AddTrack(int key, string filePath)
        {
            playlist.Add(key, filePath);
        }

        public void RemoveTrack(int key)
        {
            playlist.Remove(key);
        }

        public RepeatMode CurrentRepeatMode { get; private set; }

        public void SetRepeatMode(RepeatMode repeatMode)
        {
            CurrentRepeatMode = repeatMode;
            if (repeatMode == RepeatMode.CurrentFile)
            {
                CurrentRepeatMode = repeatMode;
                playbackSession.RepeatCurrentTrack = true;
            }
        }

        private void OnPlaybackEnded()
        {
            if (CurrentRepeatMode == RepeatMode.CurrentFile)
            {
                playbackSession.RepeatCurrentTrack = true;
            }
        }
    }
}