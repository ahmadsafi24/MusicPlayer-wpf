using Helper;
using PlayerLibrary.Core;
using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using static PlayerLibrary.Events;

namespace PlayerLibrary.Shell
{
    public class TimelineController
    {
        private PlaybackSession playbackSession; //remove it
        private NAudioCore nAudioCore;

        internal TimelineController(PlaybackSession playbackSession)
        {
            this.playbackSession = playbackSession;
            this.nAudioCore = playbackSession.nAudioCore;
            InitializeTimers();
            playbackSession.PlaybackStateChanged += PlaybackStateChanged;
        }

        internal TimelineController(NAudioCore nAudioCore)
        {
            this.nAudioCore = nAudioCore;
            InitializeTimers();
        }
        public TimeSpan Current
        {
            get
            {
                if (nAudioCore.Reader != null)
                {
                    return nAudioCore.Reader.CurrentTime;
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
                if (nAudioCore.Reader != null)
                {
                    return nAudioCore.Reader.TotalTime;
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
                if (time < TimeSpan.Zero || time > Total)
                { return; }

                nAudioCore.Reader.CurrentTime = time;
                InvokeCurrentTime(time);
                Log.WriteLine($"Seeking (async) to {time.ToString("mm\\:ss")}");

            });
        }

        private readonly DispatcherTimer CurrentTimeWatcher = new();
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
                CurrentTimeWatcher.Start();
            }
            else
            {
                CurrentTimeWatcher.Stop();
            }
        }

        private void CurrentTimeWatcher_Tick(object sender, EventArgs e)
        {
            InvokeCurrentTime(Current);
        }

        public event EventHandlerTimeSpan TimePositionChanged;
        internal async void InvokeCurrentTime(TimeSpan timespan)
        {
            await Task.Run(() => TimePositionChanged?.Invoke(timespan));

        }
    }
}