namespace PlayerUI.Commands
{
    public static class WindowTheme
    {
        public delegate void EventHandlerThemeChanged(bool isDark);
        public static event EventHandlerThemeChanged ThemeChanged;

        public static void FireThemeChangedForWindows()
        {
            ThemeChanged?.Invoke(Statics.IsDark);
        }

        public static void DarkThemeToggle()
        {
            if (Statics.IsDark)
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
            Statics.IsDark = true;
            FireThemeChangedForWindows();
            ResourceManager.LoadThemeResourceDark();
        }

        public static void ForceApplyLight()
        {
            Statics.IsDark = false;
            FireThemeChangedForWindows();
            ResourceManager.LoadThemeResourceLight();
        }

        public static void Refresh()
        {
            if (Statics.IsDark)
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
