using Helper.DarkUi;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Helper
{
    public static class ThemeListener
    {
        public static bool RegistryisDark()
        {
            const string PersonalizeRegistryKeyPath = @"Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";
            const string UseLightThemeRegistryKey = "AppsUseLightTheme";
            Microsoft.Win32.RegistryKey registry = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(PersonalizeRegistryKeyPath);
            string islighttheme = registry.GetValue(UseLightThemeRegistryKey).ToString();
            return islighttheme != "1";//means valueisdark
        }
    }
}

