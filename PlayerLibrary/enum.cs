namespace PlayerLibrary
{
    public enum RepeatMode
    {
        Stop = 0,
        Close = 1,
        CurrentFile = 2,
        NextFile = 3,
        CurrentPlaylist = 4
    }
    public enum PlaybackState
    {
        None = -3,
        Unknown = -2,
        Failed = -1,
        Opened = 0,
        Paused = 1,
        Playing = 2,
        Stopped = 3,
        Ended = 4,
        Closed = 5
    }
    public enum EqualizerMode
    {
        Normal = 0,
        Super = 1
    }
    public enum CoreType
    {
        Normal = 0,
        WithEqualizer = 1
    }
}
