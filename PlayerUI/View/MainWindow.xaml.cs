using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.Windows.ApplicationModel.DynamicDependency;
using PlayerUI.Pages;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Shell;

namespace PlayerUI.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Initialized += MainWindow_Initialized;
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;

            WindowTheme.ThemeChanged += WindowTheme_ThemeChanged;
            WindowCommands.AttachDrop(this);
            WindowCommands.AttachMouseWheel(this);

        }

        private  void MainWindow_Initialized(object sender, EventArgs e)
        {


        }

        private static async void AnimateBackgoundOpacity(Window window)
        {
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                SolidColorBrush winbackcolor = new(((SolidColorBrush)window.Background).Color);
                Brush brush = winbackcolor;
                window.Background = brush;

                DoubleAnimation doubleanimation = new(0, new Duration(TimeSpan.FromSeconds(0.5)), FillBehavior.HoldEnd);
                brush.BeginAnimation(SolidColorBrush.OpacityProperty, doubleanimation);

                //ColorAnimation coloAnimotion = new(winbackcolor.Color, System.Windows.Media.Colors.Transparent, new Duration(TimeSpan.FromSeconds(8)), FillBehavior.HoldEnd);
                //brush.BeginAnimation(SolidColorBrush.ColorProperty, coloAnimotion);

            }));

        }

        private static async void ApplyWindowchrome(Window window)
        {
            await Task.Delay(10);
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                WindowChrome chrome = new()
                {
                    CaptionHeight = 35,
                    ResizeBorderThickness = new Thickness(5),
                    //CornerRadius = new CornerRadius(5),
                    GlassFrameThickness = new Thickness(-1),
                    UseAeroCaptionButtons = true,
                    NonClientFrameEdges = NonClientFrameEdges.Right

                };
                WindowChrome.SetWindowChrome(window, chrome);
                MainPage content = window.Content as MainPage;
                content.Margin = new Thickness(0, 0, 4, 0);
            }));

        }

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState != WindowState.Maximized)
            {
                WindowsManager.DragMove(this);
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            AppStatics.IsWindowStateMaximized = (WindowState == WindowState.Maximized);
            if (WindowState == WindowState.Normal)
            {
                AppStatics.WindowsWidth = ActualWidth;
                AppStatics.WindowsHeight = ActualHeight;
                AppStatics.WindowsLeft = Left;
                AppStatics.WindowsTop = Top;
            }
            appWindow.Destroy();
            Bootstrap.Shutdown();

        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppStatics.IsWindowStateMaximized == false)
            {
                Width = AppStatics.WindowsWidth;
                Height = AppStatics.WindowsHeight;
                Left = AppStatics.WindowsLeft;
                Top = AppStatics.WindowsTop;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }

            await Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                Content = FindResource("MainPage");
            }));
            ApplyWindowchrome(this);
            ApplyMica(this);

            await Task.Delay(100);
            AnimateBackgoundOpacity(this);

        }

        private static void ApplyMica(Window window)
        {

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
             {
                 IntPtr intPtr = new WindowInteropHelper(window).Handle;
                 _ = Dwm.DwmSetWindowAttribute(intPtr, Dwm.DwmWindowAttribute.DWMWA_MICA_EFFECT, ref Dwm.trueValue, Marshal.SizeOf(typeof(int)));
             }));

        }


        private void WindowTheme_ThemeChanged(bool isdark)
        {
            if (IsLoaded)
            {
                SetDarkMode(this, AppStatics.IsDark);
            }
            if (appWindow != null)
            {
                ApplyTitlebarColor(appWindow);
            }
            UpdateLayout();

        }

        private static void ApplyTitlebarColor(AppWindow appWindow)
        {
                //System.Drawing.Color BackgroundColor = System.Drawing.ColorTranslator.FromHtml(App.Current.FindResource("AppWindow.Active.Background").ToString());
                System.Drawing.Color ForegroundColor = System.Drawing.ColorTranslator.FromHtml(App.Current.FindResource("AppWindow.Active.Foreground").ToString());

                //System.Drawing.Color InactiveBackgroundColor = System.Drawing.ColorTranslator.FromHtml(App.Current.FindResource("AppWindow.InActive.Background").ToString());
                System.Drawing.Color InactiveForegroundColor = System.Drawing.ColorTranslator.FromHtml(App.Current.FindResource("AppWindow.InActive.Foreground").ToString());

                System.Drawing.Color HoverBackgroundColor = System.Drawing.ColorTranslator.FromHtml(App.Current.FindResource("AppWindow.Hover.Background").ToString());
                System.Drawing.Color HoverForegroundColor = System.Drawing.ColorTranslator.FromHtml(App.Current.FindResource("AppWindow.Hover.Foreground").ToString());

                appWindow.TitleBar.BackgroundColor = UiColorConverter.Transparent;//  BackgroundColor.ToUiColor();
                appWindow.TitleBar.ForegroundColor = ForegroundColor.ToUiColor();
                appWindow.TitleBar.InactiveBackgroundColor = UiColorConverter.Transparent;// InactiveBackgroundColor.ToUiColor();
                appWindow.TitleBar.InactiveForegroundColor = InactiveForegroundColor.ToUiColor();

                appWindow.TitleBar.ButtonBackgroundColor = UiColorConverter.Transparent;// BackgroundColor.ToUiColor();
                appWindow.TitleBar.ButtonForegroundColor = ForegroundColor.ToUiColor();
                appWindow.TitleBar.ButtonInactiveBackgroundColor = UiColorConverter.Transparent;// InactiveBackgroundColor.ToUiColor();
                appWindow.TitleBar.ButtonInactiveForegroundColor = InactiveForegroundColor.ToUiColor();

                appWindow.TitleBar.ButtonHoverBackgroundColor = HoverBackgroundColor.ToUiColor();
                appWindow.TitleBar.ButtonHoverForegroundColor = HoverForegroundColor.ToUiColor();
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            SetDarkMode(this, AppStatics.IsDark);

            bool success = Bootstrap.TryInitialize(0x00010000, out _);

            WindowInteropHelper intptr = new(this);
            WindowId winid = Win32Interop.GetWindowIdFromWindow(intptr.Handle);
            appWindow = AppWindow.GetFromWindowId(winid);
            appWindow.TitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
            ApplyTitlebarColor(appWindow);

            appWindow.TitleBar.ExtendsContentIntoTitleBar = true;

            appWindow.TitleBar.SetDragRectangles(new[]
            {
                new Windows.Graphics.RectInt32(0,0,0,0)
            });
        }

        private static void SetDarkMode(Window window, bool isDark)
        {
            DwmApi.ToggleImmersiveDarkMode(window, AppStatics.IsDark);
            AppMode appMode;
            if (AppStatics.IsDark == true)
            {
                appMode = AppMode.ForceDark;
            }
            else
            {
                appMode = AppMode.ForceLight;
            }
            Helper.DarkUi.Win32.SetPreferredAppMode(appMode);
        }

        private AppWindow appWindow;
    }


    public static class UiColorConverter
    {
        public static Windows.UI.Color ToUiColor(this System.Drawing.Color color)
        {
            return new Windows.UI.Color() { A = color.A, R = color.R, G = color.G, B = color.B };
        }

        public static Windows.UI.Color CreateTransparent(this Windows.UI.Color nullcolor)
        {
            return new Windows.UI.Color() { A = 0, R = 0, G = 0, B = 0 };
        }

        public static Windows.UI.Color Transparent = new Windows.UI.Color().CreateTransparent();
    }

    internal static class Dwm
    {
        [DllImport("dwmapi.dll")]
        internal static extern int DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttribute dwAttribute, ref int pvAttribute, int cbAttribute);

        [Flags]
        internal enum DwmWindowAttribute : uint
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
            DWMWA_MICA_EFFECT = 1029
        }

        internal static int trueValue = 0x01;
        internal static int falseValue = 0x00;
    }
}
