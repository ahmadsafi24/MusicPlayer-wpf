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
            Theme.WindowTheme.ThemeChanged += WindowTheme_ThemeChanged;
            MouseLeftButtonDown += (_, _) => Helper.WindowsManager.DragMove(this);
            Commands.Window.AttachDrop(this);
            Commands.Window.AttachMouseWheel(this);
        }

        private void WindowTheme_ThemeChanged(bool isdark)
        {
            DwmApi.ToggleImmersiveDarkMode(this, isdark);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            Helper.ControlboxHelper.RemoveControls(this);
            Helper.IconHelper.RemoveIcon(this);
            DwmApi.ToggleImmersiveDarkMode(this, Theme.WindowTheme.IsDark);
        }
    }
}
