using Engine.Events.Base;
using Engine.Internal;

namespace Engine.Events
{
    public static class AllEvents
    {
        public static event EventHandlerPlaybackState PlaybackStateChanged;
        public static event EventHandlerTimeSpan CurrentTimeChanged;
        public static event EventHandlerNull VolumeChanged;

        internal static void InvokeCurrentTime() => _ = (CurrentTimeChanged?.Invoke(Player.CurrentTime));

        internal static async void InvokePlaybackStateChanged(Enums.PlaybackState value)
        {
            await PlaybackStateChanged?.Invoke(value);
        }

        internal static async void InvokeVolumeChanged() => await VolumeChanged?.Invoke();
    }
}
