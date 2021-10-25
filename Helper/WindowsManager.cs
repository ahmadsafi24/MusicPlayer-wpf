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



        public static void EnableBlur(Window window)
        {
            var windowHelper = new WindowInteropHelper(window);

            var accent = new Native.AccentPolicy
            {
                AccentState = Native.AccentState.ACCENT_ENABLE_BLURBEHIND
            };

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new Native.WindowCompositionAttributeData
            {
                Attribute = Native.WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref Native.WindowCompositionAttributeData data);


    }
}
namespace Native
{
    internal enum AccentState
    {
        ACCENT_DISABLED,
        ACCENT_ENABLE_GRADIENT,
        ACCENT_ENABLE_TRANSPARENTGRADIENT,
        ACCENT_ENABLE_BLURBEHIND,
        ACCENT_INVALID_STATE,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    internal enum WindowCompositionAttribute
    {
        // 省略其他未使用的字段
        WCA_ACCENT_POLICY = 19,
        // 省略其他未使用的字段
    }
}