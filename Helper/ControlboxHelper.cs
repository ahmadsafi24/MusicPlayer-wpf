using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Helper
{
    public static class ControlboxHelper
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public static void RemoveControls(Window window)
        {
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            _ = SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }
    }
}
