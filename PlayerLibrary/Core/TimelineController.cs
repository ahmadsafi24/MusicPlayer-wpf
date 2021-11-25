using Helper;
using PlayerLibrary.Core;
using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using static PlayerLibrary.Events;

namespace PlayerLibrary.Core
{
    public class TimelineController
    {
        internal PlaybackSession PlaybackSession { get; set; }

        internal TimelineController(PlaybackSession playbackSession)
        {
            Log.WriteLine("new TimelineController");
            this.PlaybackSession = playbackSession;
            InitializeTimers();
            playbackSession.PlaybackStateChanged += PlaybackStateChanged;
        }

        public TimeSpan Current
        {
            get
            {
                if (PlaybackSession.audioPlayer.Reader != null)
                {
                    return PlaybackSession.audioPlayer.Reader.CurrentTime;
                }
                else
                {
                    return TimeSpan.Zero;
                }

            }
        }

        public TimeSpan Total
        {
            get
            {
                if (PlaybackSession.audioPlayer.Reader != null)
                {
                    return PlaybackSession.audioPlayer.Reader.TotalTime;
                }
                else
                {
                    return TimeSpan.FromSeconds(0);
                }
            }
        }

        public async Task SeekAsync(TimeSpan time)
        {
            await Task.Run(() =>
            {
                Seek(time);
            });
        }

        public void Seek(TimeSpan time)
        {
            if (time < TimeSpan.Zero || time > Total || time == Current)
            { return; }

            PlaybackSession.audioPlayer.Reader.CurrentTime = time;
            Task.Run(async () => await RaiseCurrentTime(Current));
            Log.WriteLine($"Seeking (async) to {time.ToString("mm\\:ss")}");
        }

        public readonly DispatcherTimer CurrentTimeWatcher = new();
        private void InitializeTimers()
        {
            CurrentTimeWatcher.Interval = TimeSpan.FromSeconds(0.5);
            CurrentTimeWatcher.Tick += CurrentTimeWatcher_Tick;
            CurrentTimeWatcher.Start();
        }

        private void PlaybackStateChanged(PlaybackState playbackState)
        {
            if (playbackState == PlaybackState.Playing)
            {
                Log.WriteLine("timeline timer started");
                CurrentTimeWatcher.Start();
            }
            else
            {
                Log.WriteLine("timeline timer stoped");
                CurrentTimeWatcher.Stop();
            }
        }

        private void CurrentTimeWatcher_Tick(object sender, EventArgs e)
        {
            Task.Run(async () => await RaiseCurrentTime(Current));
        }

        public event EventHandlerTimeSpan TimePositionChanged;
        internal async Task RaiseCurrentTime(TimeSpan timespan)
        {
            await Task.Run(() => TimePositionChanged?.Invoke(timespan));

        }
    }
}