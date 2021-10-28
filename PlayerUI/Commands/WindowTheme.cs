using PlayerUI.Config;

namespace PlayerUI.Commands
{
    public static class WindowTheme
    {
        public delegate void EventHandlerThemeChanged(bool isDark);
        public static event EventHandlerThemeChanged ThemeChanged;

        public static void FireThemeChangedForWindows()
        {
            ThemeChanged?.Invoke(AppStatics.IsDark);
        }

        public static void DarkThemeToggle()
        {
            if (AppStatics.IsDark)
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
            AppStatics.IsDark = true;
            FireThemeChangedForWindows();
            ResourceManager.LoadThemeResourceDark();
        }

        public static void ForceApplyLight()
        {
            AppStatics.IsDark = false;
            FireThemeChangedForWindows();
            ResourceManager.LoadThemeResourceLight();
        }

        public static void Refresh()
        {
            if (AppStatics.IsDark)
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
