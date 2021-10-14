using Helper.DarkUi;
using PlayerUI.Theme;
using System;
using System.Windows;
namespace PlayerUI.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MouseLeftButtonDown += (_, _) =>
            {
                if (this.WindowState != WindowState.Maximized)
                {
                    Helper.WindowsManager.DragMove(this);
                }
            };

            WindowTheme.ThemeChanged += WindowTheme_ThemeChanged;
            Commands.Window.AttachDrop(this);
            Commands.Window.AttachMouseWheel(this);
        }

        private void WindowTheme_ThemeChanged(bool isdark)
        {
            if (this.IsLoaded)
            {
                DwmApi.ToggleImmersiveDarkMode(this, isdark);
                UpdateLayout();
                Hide();
                Show();
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            Helper.IconHelper.RemoveIcon(this);
            DwmApi.ToggleImmersiveDarkMode(this, WindowTheme.IsDark);
        }
    }
}
