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
                    Commands.WindowTheme.IsDark = true;
                    Commands.ResourceManager.LoadThemeResourceDark();
                }
                else
                {
                    Commands.WindowTheme.IsDark = false;
                    Commands.ResourceManager.LoadThemeResourceLight();
                }

            }
            catch (System.Exception ex)
            {
                _ = MessageBox.Show(ex.Message);
            }
        }
    }
}
