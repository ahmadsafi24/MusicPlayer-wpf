using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace MusicApplication.Setting
{
    public static class Load
    {
        public static async void LoadIsDark()
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
                    await Task.Run(() => Theme.ResourceManager.LoadThemeResourceDark());
                }
                else
                {
                    Theme.WindowTheme.IsDark = false;
                    await Task.Run(() => Theme.ResourceManager.LoadThemeResourceLight());
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
