namespace AudioPlayer
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
        NormalEqualizer8band = 0,
        SuperEqualizer16band = 1
    }
}
