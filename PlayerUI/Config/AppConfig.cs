using System.IO;
using System.Windows;

namespace PlayerUI.Config
{
    public static class AppConfig
    {
        public static string ConfigFilePath { get; set; }
        public static ConfigModel CurrentConfig { get; set; } = new();

        public static ConfigModel DefaultConfig => CreateDefaultConfig();

        private static ConfigModel CreateDefaultConfig()
        {
            ConfigModel config = new()
            {
                LastFile = null,
                IsDark = false,
                WindowsWidth = 800,
                WindowsHeight = 600,
                WindowsLeft = PlayerUI.App.Current.MainWindow.Left,
                WindowsTop = PlayerUI.App.Current.MainWindow.Top,
                EqBandsGain=new[]{0,0,0,0,0,0,0,0,0,0,0}
            };
            return config;
        }

        public static void Initialize()
        {
            ConfigFilePath = System.AppContext.BaseDirectory + @"\Config.json";
            App.Player.PlaybackStateChanged += Player_PlaybackStateChanged;
        }

        private static void Player_PlaybackStateChanged(PlayerLibrary.PlaybackState playbackState)
        {
            if (playbackState == PlayerLibrary.PlaybackState.Opened)
            {
                AppStatics.LastFile = App.Player.Source;
            }
        }

        /// <summary>
        /// Saves Current Config As JsonFile
        /// </summary>
        public static void WriteConfig()
        {
            using var ms = new MemoryStream();
            using var writer = new System.Text.Json.Utf8JsonWriter(ms,new System.Text.Json.JsonWriterOptions(){Indented=true});

            writer.WriteStartObject();
            writer.WriteString(nameof(ConfigModel.LastFile), CurrentConfig.LastFile);
            writer.WriteBoolean(nameof(ConfigModel.IsDark), CurrentConfig.IsDark);
            writer.WriteNumber(nameof(ConfigModel.WindowsWidth), (int)CurrentConfig.WindowsWidth);
            writer.WriteNumber(nameof(ConfigModel.WindowsHeight), (int)CurrentConfig.WindowsHeight);
            writer.WriteNumber(nameof(ConfigModel.WindowsLeft), (int)CurrentConfig.WindowsLeft);
            writer.WriteNumber(nameof(ConfigModel.WindowsTop), (int)CurrentConfig.WindowsTop);
            writer.WriteStartArray(nameof(ConfigModel.EqBandsGain));
            foreach (var item in CurrentConfig.EqBandsGain)
            {
                writer.WriteNumberValue(item);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
            writer.Flush();
            File.WriteAllBytes(ConfigFilePath, ms.ToArray());
        }

        /// <summary>
        /// Loads JsonFile to Current Config 
        /// </summary>
        public static void ReadConfig()
        {
            if (File.Exists(ConfigFilePath))
            {
                var json = File.ReadAllBytes(ConfigFilePath);
                ConfigModel Conf = System.Text.Json.JsonSerializer.Deserialize<ConfigModel>(json);
                if (Conf != null)
                    CurrentConfig = Conf;
            }
            else
            {
                
                MessageBox.Show("Config Not Found \n Ok To Create Default Config");
                CurrentConfig = DefaultConfig;
            }
            try
            {
                
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        public static void LoadAppConfigs()
        {
            AppConfig.Initialize();
            AppConfig.ReadConfig();

            AppStatics.IsDark = AppConfig.CurrentConfig.IsDark;
            AppStatics.LastFile = AppConfig.CurrentConfig.LastFile;
            AppStatics.WindowsLeft = AppConfig.CurrentConfig.WindowsLeft;
            AppStatics.WindowsTop = AppConfig.CurrentConfig.WindowsTop;
            AppStatics.WindowsWidth = AppConfig.CurrentConfig.WindowsWidth;
            AppStatics.WindowsHeight = AppConfig.CurrentConfig.WindowsHeight;

            if (CurrentConfig.EqBandsGain.Length < 10)
            {
                App.Player.EqualizerMode = PlayerLibrary.EqualizerMode.Normal;

            }
            else if (CurrentConfig.EqBandsGain.Length > 10)
            {
                App.Player.EqualizerMode = PlayerLibrary.EqualizerMode.Super;
            }
            App.Player.ReIntialEq();
            int i = 0;
            foreach (var item in CurrentConfig.EqBandsGain)
            {
                App.Player.ChangeEq(i, (float)item,true);
                i++;
            }
            Commands.WindowTheme.Refresh();
        }

        public static void SaveAppConfigs()
        {
            try
            {
            AppConfig.CurrentConfig = new()
            {
                LastFile = AppStatics.LastFile,
                IsDark = AppStatics.IsDark,
                WindowsWidth = AppStatics.WindowsWidth,
                WindowsHeight = AppStatics.WindowsHeight,
                WindowsLeft = AppStatics.WindowsLeft,
                WindowsTop = AppStatics.WindowsTop,

                EqBandsGain = App.Player.EqBandsGain
            };
            AppConfig.WriteConfig();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

    }
}

/*public static Config CreateNewConfig(string filePath, bool isDark)
{
var conf = new Config(filePath, isDark);
return conf;
}*/