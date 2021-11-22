namespace PlayerUI.Common.Commands
{
    internal class Taskbar
    {
        private static int lastValue = 0;

        private static void SetTaskbarValue(int val)
        {
            if (val != lastValue)
            {
                Progress.SetValue(val, 100, true);
            }

            lastValue = val;
        }
        public static void SetTaskbarProgressValue(TimeSpan currentTime, TimeSpan maxTime)
        {
            if (maxTime <= TimeSpan.FromSeconds(10))
            {
                SetTaskbarValue(0);
                return;
            }
            int percentValue = (int)maxTime.TotalSeconds / 100;
            if (percentValue == 0)
            {
                SetTaskbarValue(0);
                return;
            }
            int currentValue = (int)currentTime.TotalSeconds / percentValue;
            SetTaskbarValue(currentValue);
        }

        public static void SetTaskbarState(Helper.Taskbar.ProgressState progressState)
        {
            Helper.Taskbar.Progress.SetState(progressState, true);
        }
    }
}
