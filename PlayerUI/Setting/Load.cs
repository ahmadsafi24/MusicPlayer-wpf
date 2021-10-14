using System.IO;
using System.Windows;

namespace PlayerUI.Setting
{
    public static class Load
    {
        public static void LoadIsDark()
        {
            try
            {
                bool isdark;
                string setting_isdarkPath = @"Setting\IsDark";
                if (File.Exists(setting_isdarkPath)) { string value = File.ReadAllText(setting_isdarkPath); isdark = bool.Parse(value); }
                else
                { isdark = Helper.CustomThemeListener.IsDark; }
                if (isdark)
                {
                    Theme.WindowTheme.IsDark = true;
                    Theme.ResourceManager.LoadThemeResourceDark();
                }
                else
                {
                    Theme.WindowTheme.IsDark = false;
                    Theme.ResourceManager.LoadThemeResourceLight();
                }

            }
            catch (System.Exception ex)
            {
                _ = MessageBox.Show(ex.Message);
            }
        }
    }
}
