namespace PlayerUI.Commands
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
            IsDark = true;
            FireThemeChangedForWindows();
            ResourceManager.LoadThemeResourceDark();
        }

        public static void ForceApplyLight()
        {
            IsDark = false;
            FireThemeChangedForWindows();
            ResourceManager.LoadThemeResourceLight();
        }

        public static void Refresh()
        {
            if (IsDark)
            {
                ForceApplyDark();
            }
            else
            {
                ForceApplyLight();
            }
        }

    }
}
