namespace PlayerUI.Theme
{
    public static class WindowTheme
    {
        public delegate void EventHandlerThemeChanged(bool isdark);
        public static event EventHandlerThemeChanged ThemeChanged;

        public static bool IsDark { get; set; } 

        public static void FireThemeChangedForWindows()
        {
            ThemeChanged?.Invoke(IsDark);
        }

        public static void DarkThemeToggle()
        {
            if (IsDark)
            {
                ForceApplyLight();
            }
            else
            {
                ForceApplyDark();
            }
        }

        public static void ForceApplyDark()
        {
            ResourceManager.LoadThemeResourceDark();
            IsDark = true;
            FireThemeChangedForWindows();
        }

        public static void ForceApplyLight()
        {
            ResourceManager.LoadThemeResourceLight();
            IsDark = false;
            FireThemeChangedForWindows();
        }
    }
}
