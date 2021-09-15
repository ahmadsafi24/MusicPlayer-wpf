using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Helper
{
    public sealed class WindowsManager
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        public static void DragMove(Window window)
        {
            WindowInteropHelper helper = new(window);
            _ = SendMessage(helper.Handle, 161, 2, 0);
        }

        public static void CloseWindow(Window window)
        {
            SystemCommands.CloseWindow(window);
        }

        public static void MaximizeRestore(Window window)
        {
            switch (window.WindowState)
            {
                case WindowState.Normal:
                    SystemCommands.MaximizeWindow(window);
                    break;
                case WindowState.Minimized:
                    SystemCommands.RestoreWindow(window);
                    break;
                case WindowState.Maximized:
                    SystemCommands.RestoreWindow(window);
                    break;
                default:
                    break;
            }
        }
        public static void Minimize(Window window)
        {
            SystemCommands.MinimizeWindow(window);
        }
        public static void ShowContextMenu(Window window)
        {
            double posleft = 0;
            double postop = 0;
            if (window.WindowState == WindowState.Normal)
            {
                posleft = window.Left;
                postop = window.Top;
            }

            Point point = new(Mouse.GetPosition(window).X + posleft, Mouse.GetPosition(window).Y + postop);
            SystemCommands.ShowSystemMenu(window, point);
        }
    }
}
