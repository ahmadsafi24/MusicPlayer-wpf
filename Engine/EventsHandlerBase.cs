using System;

namespace Engine
{
    public delegate void EventHandlerTimeSpan(TimeSpan Time);
    public delegate void EventHandlerPlaybackState(Enums.PlaybackState newPlaybackState);
    public delegate void EventHandlerEmpty();
}
