using System.IO;
using System.Windows;

namespace PlayerUI
{
    public class Config
    {
        public string LastFile { get; set; } = string.Empty;
        public bool IsDark { get; set; } = false;
        public double WindowsWidth { get; set; }
        public double WindowsHeight { get; set; }
        public double WindowsLeft { get; set; }
        public double WindowsTop { get; set; }
    }

    public static class Settings
    {
        public static string ConfigFilePath { get; set; }
        public static Config CurrentConfig { get; set; } = new();

        public static void Initialize()
        {
            ConfigFilePath = System.AppContext.BaseDirectory + @"\Config.json";
            App.Player.PlaybackStateChanged += Player_PlaybackStateChanged;
        }

        private static void Player_PlaybackStateChanged(PlayerLibrary.PlaybackState playbackState)
        {
            if (playbackState == PlayerLibrary.PlaybackState.Opened)
            {
                Statics.LastFile = App.Player.Source;
            }
        }

        /// <summary>
        /// Saves Current Config As JsonFile
        /// </summary>
        public static void SaveConfig()
        {
            using var ms = new MemoryStream();
            using var writer = new System.Text.Json.Utf8JsonWriter(ms);

            writer.WriteStartObject();
            writer.WriteString(nameof(Config.LastFile), CurrentConfig.LastFile);
            writer.WriteBoolean(nameof(Config.IsDark), CurrentConfig.IsDark);
            writer.WriteNumber(nameof(Config.WindowsWidth), CurrentConfig.WindowsWidth);
            writer.WriteNumber(nameof(Config.WindowsHeight), CurrentConfig.WindowsHeight);
            writer.WriteNumber(nameof(Config.WindowsLeft), CurrentConfig.WindowsLeft);
            writer.WriteNumber(nameof(Config.WindowsTop), CurrentConfig.WindowsTop);
            writer.WriteEndObject();
            writer.Flush();
            File.WriteAllBytes(ConfigFilePath, ms.ToArray());
        }

        /// <summary>
        /// Loads JsonFile to Current Config 
        /// </summary>
        public static void LoadConfig()
        {
            if (!File.Exists(ConfigFilePath))
            {
                MessageBox.Show("Config File Not Exists!");
                return;
            }
            var json = File.ReadAllBytes(ConfigFilePath);
            try
            {
                Config Conf = System.Text.Json.JsonSerializer.Deserialize<Config>(json);
                if (Conf != null)
                    CurrentConfig = Conf;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }


        }
    }

    public static class SettingLoader
    {
        public static void LoadAppSettings()
        {
            Settings.Initialize();
            Settings.LoadConfig();

            Statics.IsDark = Settings.CurrentConfig.IsDark;
            Statics.LastFile = Settings.CurrentConfig.LastFile;
            Statics.WindowsLeft = Settings.CurrentConfig.WindowsLeft;
            Statics.WindowsTop = Settings.CurrentConfig.WindowsTop;
            Statics.WindowsWidth = Settings.CurrentConfig.WindowsWidth;
            Statics.WindowsHeight = Settings.CurrentConfig.WindowsHeight;

            Commands.WindowTheme.Refresh();
        }

        public static void SaveAppSettings()
        {
            Settings.CurrentConfig = new()
            {
                LastFile = Statics.LastFile,
                IsDark = Statics.IsDark,
                WindowsWidth = Statics.WindowsWidth,
                WindowsHeight = Statics.WindowsHeight,
                WindowsLeft = Statics.WindowsLeft,
                WindowsTop = Statics.WindowsTop
            };
            Settings.SaveConfig();
        }
    }

    public static class Statics
    {
        public static string LastFile { get; set; }
        public static bool IsDark { get; set; }
        public static double WindowsWidth { get; set; }
        public static double WindowsHeight { get; set; }
        public static double WindowsLeft { get; set; }
        public static double WindowsTop { get; set; }
    }
}

/*public static Config CreateNewConfig(string filePath, bool isDark)
{
var conf = new Config(filePath, isDark);
return conf;
}*/