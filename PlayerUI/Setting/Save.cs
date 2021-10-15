using System;
using System.Configuration;
using System.IO;

namespace PlayerUI.Setting
{
    public static class Save
    {
        public static void SaveIsDark()
        {
            Directory.CreateDirectory(@"Setting\");
            File.WriteAllText(@"Setting\IsDark", Commands.WindowTheme.IsDark.ToString());

            SettingsProperty settingsIsDark = new("isdark");
            settingsIsDark.DefaultValue = "False";
            Properties.Settings.Default.Properties.Add(settingsIsDark);
            Properties.Settings.Default.Save();
        }
    }
}
