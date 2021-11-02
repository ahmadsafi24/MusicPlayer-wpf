using Helper.DarkUi;
using PlayerUI.Config;
using System;
using System.Windows;

namespace PlayerUI.Windows
{
    public partial class ControlbarWindows : Window
    {
        public ControlbarWindows()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.Manual;
            SizeChanged += ControlbarWindows_SizeChanged;
            Commands.WindowTheme.ThemeChanged += WindowTheme_ThemeChanged;
            MouseLeftButtonDown += (_, _) => Helper.WindowsManager.DragMove(this);
            Commands.Window.AttachDrop(this);
            Commands.Window.AttachMouseWheel(this);

            if (MiniViewLeft + MiniViewTop + MiniViewWidth == 0)
            {
                var warea = SystemParameters.WorkArea;
                Left = warea.Width - Width;
                Top = warea.Height - Height;
            }
            else
            {
                Left = MiniViewLeft;
                Top = MiniViewTop;
                Width = MiniViewWidth;
            }
        }

        private void WindowTheme_ThemeChanged(bool isdark)
        {
            if (this.IsLoaded)
            {
                DwmApi.ToggleImmersiveDarkMode(this, isdark);
                UpdateLayout();
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            Helper.ControlboxHelper.RemoveControls(this);
            Helper.IconHelper.RemoveIcon(this);
            DwmApi.ToggleImmersiveDarkMode(this, AppStatics.IsDark);
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            MiniViewLeft = Left;
            MiniViewTop = Top;
        }

        private void ControlbarWindows_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MiniViewWidth = ActualWidth;
        }
        public static double MiniViewTop { get; set; }
        public static double MiniViewLeft { get; set; }
        public static double MiniViewWidth { get; set; }
    }
}
