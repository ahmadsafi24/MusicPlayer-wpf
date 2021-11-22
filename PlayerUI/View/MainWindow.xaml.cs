namespace PlayerUI.View
{
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
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

            WindowTheme.ThemeChanged += WindowTheme_ThemeChanged;
            Common.Commands.WindowCommands.AttachDrop(this);
            Common.Commands.WindowCommands.AttachMouseWheel(this);
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            AppStatics.IsWindowStateMaximized = (WindowState == WindowState.Maximized);
            if (WindowState == WindowState.Normal)
            {
                AppStatics.WindowsLeft = Left;
                AppStatics.WindowsTop = Top;
                AppStatics.WindowsWidth = ActualWidth;
                AppStatics.WindowsHeight = ActualHeight;
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppStatics.IsWindowStateMaximized == false)
            {
                Left = AppStatics.WindowsLeft;
                Top = AppStatics.WindowsTop;
                Width = AppStatics.WindowsWidth;
                Height = AppStatics.WindowsHeight;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
            _ = App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                Content = App.Current.FindResource("MainPage");
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
            Helper.IconHelper.RemoveIcon(this);
            DwmApi.ToggleImmersiveDarkMode(this, AppStatics.IsDark);
        }
    }
}
