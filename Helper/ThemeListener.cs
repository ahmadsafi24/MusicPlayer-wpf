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
            return islighttheme != "1";
        }
    }
}

