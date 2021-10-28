using Helper.DarkUi;
using PlayerUI.Config;
using System;
using System.Windows;
using System.Windows.Threading;

namespace PlayerUI.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow_Chromeless.xaml
    /// </summary>
    public partial class MainWindowChromeless : Window
    {
        public MainWindowChromeless()
        {
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
            InitializeComponent();
            MouseLeftButtonDown += (_, _) =>
            {
                if (this.WindowState != WindowState.Maximized)
                {
                    Helper.WindowsManager.DragMove(this);
                }
            };

            Commands.WindowTheme.ThemeChanged += WindowTheme_ThemeChanged;
            Commands.Window.AttachDrop(this);
            Commands.Window.AttachMouseWheel(this);
        }
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            AppStatics.WindowsLeft = Left;
            AppStatics.WindowsTop = Top;
            AppStatics.WindowsWidth = ActualWidth;
            AppStatics.WindowsHeight = ActualHeight;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Left = AppStatics.WindowsLeft;
            Top = AppStatics.WindowsTop;
            Width = AppStatics.WindowsWidth;
            Height = AppStatics.WindowsHeight;
            _ = App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                Content = App.Current.FindResource("MainView");
            }));
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
            Helper.WindowsManager.EnableBlur(this);
            Helper.IconHelper.RemoveIcon(this);
            Helper.ControlboxHelper.RemoveControls(this);
            DwmApi.ToggleImmersiveDarkMode(this, AppStatics.IsDark);
        }
    }
}
