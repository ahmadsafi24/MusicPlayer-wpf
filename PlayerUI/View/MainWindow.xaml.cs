namespace PlayerUI.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
            InitializeComponent();
            MouseLeftButtonDown += (_, _) =>
            {
                if (WindowState != WindowState.Maximized)
                {
                    WindowsManager.DragMove(this);
                }
            };

            WindowTheme.ThemeChanged += WindowTheme_ThemeChanged;
            WindowCommands.AttachDrop(this);
            WindowCommands.AttachMouseWheel(this);


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
            _ = Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                Content = FindResource("MainPage");
            }));
        }

        private void WindowTheme_ThemeChanged(bool isdark)
        {
            if (IsLoaded)
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
