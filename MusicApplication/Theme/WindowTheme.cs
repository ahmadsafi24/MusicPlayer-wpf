namespace MusicApplication.Theme
{
    public static class WindowTheme
    {
        public delegate void EventHandlerThemeChanged(bool isdark);
        public static event EventHandlerThemeChanged ThemeChanged;

        private static bool _isDark;
        public static bool IsDark
        {
            get => _isDark;
            set
            {
                _isDark = value;
                if (value)
                { ResourceManager.LoadThemeResourceDark(); }
                else
                { ResourceManager.LoadThemeResourceLight(); }
                ThemeChanged?.Invoke(value);
            }
        }

        public static void DarkThemeToggle() => IsDark = !IsDark;

        public static void ForceApplyDark() => IsDark = true;

        public static void ForceApplyLight() => IsDark = false;
    }
}
