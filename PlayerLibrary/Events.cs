using System;

namespace PlayerLibrary
{
    public static class Events
    {
        public delegate void EventHandlerTimeSpan(TimeSpan timeSpan);
        public delegate void EventHandlerPlaybackState(PlaybackState playbackState);
        public delegate void EventHandlerEmpty();
        public delegate void EventHandlerVolume(int volume);
        public delegate void EventHandlerType(Type type);
    }
}
