using System;

namespace PlayerLibrary
{
    public static class Events
    {

        public delegate void EventHandlerTimeSpan(TimeSpan Time);
        public delegate void EventHandlerPlaybackState(PlaybackState newPlaybackState);
        public delegate void EventHandlerEmpty();
        public delegate void EventHandlerVolume(int Volume);
    }
}
