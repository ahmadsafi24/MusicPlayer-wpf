using Helper.DarkUi;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Helper
{
    public static class CustomThemeListener
    {
        private static readonly DispatcherTimer Timer = new() { Interval = TimeSpan.FromSeconds(5) };
        private static Window TargetWindow;
        public static void Start(Window window)
        {
            TargetWindow = window;
            Update();
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        private static bool RegistryisDark()
        {
            const string PersonalizeRegistryKeyPath = @"Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";
            const string UseLightThemeRegistryKey = "AppsUseLightTheme";
            Microsoft.Win32.RegistryKey registry = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(PersonalizeRegistryKeyPath);
            string islighttheme = registry.GetValue(UseLightThemeRegistryKey).ToString();
            return islighttheme != "1";//means valueisdark
        }

        private static bool CurrentIsDark { get; set; } = RegistryisDark();
        private static void Timer_Tick(object sender, EventArgs e)
        {
            bool newvalue = RegistryisDark();
            if (CurrentIsDark != newvalue)
            {
                CurrentIsDark = newvalue;
                Update();
            }
        }

        private static void Update()
        {
            DwmApi.ToggleImmersiveDarkMode(TargetWindow, CurrentIsDark);
            TargetWindow.UpdateLayout();
            if (CurrentIsDark)
            {
                //ffmePlayer.App.ApplyDarkTheme();
            }
            else
            {
                //ffmePlayer.App.ApplyLightTheme();
            }
            //TargetWindow.Hide();
            //Thread.Sleep(500);
            //TargetWindow.Show();
        }

        public static void Stop()
        {
            Timer.Stop();
        }

        public static void Setmode(bool ToggleDark)
        {
            CurrentIsDark = ToggleDark;
            Update();
        }

        public static bool IsDark { get => CurrentIsDark; }
    }
}

