using Helper.DarkUi;
using System;
using System.Windows;

namespace MusicApplication.Windows
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ControlbarWindows : Window
    {
        public ControlbarWindows()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.Manual;
            SizeChanged += ControlbarWindows_SizeChanged;
            Theme.WindowTheme.ThemeChanged += WindowTheme_ThemeChanged;
            MouseLeftButtonDown += (_, _) => Helper.WindowsManager.DragMove(this);
            Commands.Window.AttachDrop(this);
            Commands.Window.AttachMouseWheel(this);

            Left = MiniViewLeft;
            Top = MiniViewTop;
            Width = MiniViewWidth;
            Height = MiniViewHeight;
        }

        private void WindowTheme_ThemeChanged(bool isdark)
        {
            if (this.IsLoaded)
            {
                DwmApi.ToggleImmersiveDarkMode(this, isdark);
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            Helper.ControlboxHelper.RemoveControls(this);
            Helper.IconHelper.RemoveIcon(this);
            DwmApi.ToggleImmersiveDarkMode(this, Theme.WindowTheme.IsDark);
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            MiniViewLeft = this.Left;
            MiniViewTop = this.Top;
        }

        private void ControlbarWindows_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MiniViewWidth = this.ActualWidth;
            MiniViewHeight = this.ActualHeight;
        }
        public static double MiniViewTop;
        public static double MiniViewLeft;
        public static double MiniViewHeight;
        public static double MiniViewWidth;
    }
}
