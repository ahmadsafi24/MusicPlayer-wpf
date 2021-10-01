using Helper.DarkUi;
using MusicApplication.Theme;
using System;
using System.Windows;
namespace MusicApplication.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Initialized += (_, _) => WindowsManager.WindowInitialized(this);
            InitializeComponent();
            WindowTheme.ThemeChanged += WindowTheme_ThemeChanged;
        }

        private void WindowTheme_ThemeChanged(bool isdark)
        {
            DwmApi.ToggleImmersiveDarkMode(this, isdark);
            UpdateLayout();
            Hide();
            Show();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            Helper.IconHelper.RemoveIcon(this);
        }
    }
}
