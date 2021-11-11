using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Helper.Taskbar
{
    public static class Progress
    {
        private static ITaskbarList _taskbarList;

        static Progress()
        {
            if (!IsSupported())
                throw new Exception("Taskbar functions not available");

            _taskbarList = (ITaskbarList)new CTaskbarList();
            _taskbarList.HrInit();
        }

        private static bool IsSupported()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT &&
                Environment.OSVersion.Version.CompareTo(new Version(6, 1)) >= 0;
        }

        public static void SetState(ProgressState state, bool dispatchInvoke = false)
        {
            if (!dispatchInvoke)
            {
                SetProgressState(state);

                return;
            }

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                SetProgressState(state);
            }));
        }

        public static void SetValue(int current, int max, bool dispatchInvoke = false)
        {
            if (!dispatchInvoke)
            {
                SetProgressValue(current, max);

                return;
            }

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                SetProgressValue(current, max);
            }));
        }

        private static void SetProgressState(ProgressState state)
        {
            // Application.Current.MainWindow.TaskbarItemInfo.ProgressState = (System.Windows.Shell.TaskbarItemProgressState) state;
            if (GetHandle() != IntPtr.Zero)
            {
                _taskbarList.SetProgressState(GetHandle(), state);
            }
        }

        private static void SetProgressValue(int current, int max)
        {
                _taskbarList.SetProgressValue(
                         GetHandle(),
                         Convert.ToUInt64(current),
                         Convert.ToUInt64(max));
            
        }

        private static IntPtr GetHandle()
        {
            Window window = Application.Current.MainWindow;
            if (window != null)
            {
                return new WindowInteropHelper(window).Handle;

            }
            else
            {
                return IntPtr.Zero;

            }
        }
    }
}
