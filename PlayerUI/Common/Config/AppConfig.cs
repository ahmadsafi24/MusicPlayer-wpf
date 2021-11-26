namespace PlayerUI.Common.Config
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
                WindowsLeft = Application.Current.MainWindow.Left,
                WindowsTop = Application.Current.MainWindow.Top,

                IsEqualizerEnabled = false,
                IsPitchShiftingEnabled = false,
                AudioVolume = 1,
                IsWindowStateMaximized = false,
                EqBandsGain = Array.Empty<int>()
            };
            return config;
        }

        public static void Initialize()
        {
            ConfigFilePath = AppContext.BaseDirectory + @"\Config.json";
            App.Player.PlaybackSession.PlaybackStateChanged += Player_PlaybackStateChanged;
        }

        private static void Player_PlaybackStateChanged(PlaybackState playbackState)
        {
            if (playbackState == PlaybackState.Opened)
            {
                AppStatics.LastFile = App.Player.PlaybackSession.CurrentTrackFile.OriginalString;
            }
        }

        /// <summary>
        /// Saves Current Config As JsonFile
        /// </summary>
        public static void WriteConfig()
        {
            using var ms = new MemoryStream();
            using var writer = new System.Text.Json.Utf8JsonWriter(ms, new System.Text.Json.JsonWriterOptions() { Indented = true });

            writer.WriteStartObject();
            writer.WriteString(nameof(ConfigModel.LastFile), CurrentConfig.LastFile);
            writer.WriteBoolean(nameof(ConfigModel.IsDark), CurrentConfig.IsDark);
            writer.WriteNumber(nameof(ConfigModel.WindowsWidth), (int)CurrentConfig.WindowsWidth);
            writer.WriteNumber(nameof(ConfigModel.WindowsHeight), (int)CurrentConfig.WindowsHeight);
            writer.WriteNumber(nameof(ConfigModel.WindowsLeft), (int)CurrentConfig.WindowsLeft);
            writer.WriteNumber(nameof(ConfigModel.WindowsTop), (int)CurrentConfig.WindowsTop);
            writer.WriteBoolean(nameof(ConfigModel.IsWindowStateMaximized), CurrentConfig.IsWindowStateMaximized);

            writer.WriteBoolean(nameof(ConfigModel.IsEqualizerEnabled), CurrentConfig.IsEqualizerEnabled);
            writer.WriteBoolean(nameof(ConfigModel.IsPitchShiftingEnabled), CurrentConfig.IsPitchShiftingEnabled);
            writer.WriteNumber(nameof(ConfigModel.AudioVolume), CurrentConfig.AudioVolume);
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
            try
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
                    _ = Log.ShowMessage("Config Not Found\nOk To Create Default Config");
                    CurrentConfig = DefaultConfig;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        public static void LoadConfigs()
        {
            Initialize();
            ReadConfig();

            AppStatics.IsDark = CurrentConfig.IsDark;
            AppStatics.LastFile = CurrentConfig.LastFile;
            AppStatics.WindowsLeft = CurrentConfig.WindowsLeft;
            AppStatics.WindowsTop = CurrentConfig.WindowsTop;
            AppStatics.WindowsWidth = CurrentConfig.WindowsWidth;
            AppStatics.WindowsHeight = CurrentConfig.WindowsHeight;
            AppStatics.IsWindowStateMaximized = CurrentConfig.IsWindowStateMaximized;

            App.Player.PlaybackSession.EffectContainer.EnableEqualizer = CurrentConfig.IsEqualizerEnabled;
            App.Player.PlaybackSession.EffectContainer.EnablePtchShifting = CurrentConfig.IsPitchShiftingEnabled;
            App.Player.PlaybackSession.VolumeController.Volume = (float)CurrentConfig.AudioVolume;
            // bad coding
            EqualizerMode newEqMode = new();
            if (CurrentConfig.EqBandsGain.Length <= 8)
            {
                newEqMode = EqualizerMode.Normal;
            }
            else if (CurrentConfig.EqBandsGain.Length >= 12)
            {
                newEqMode = EqualizerMode.Super;
            }
            EqPreset preset = new(newEqMode.ToString(), CurrentConfig.EqBandsGain);
            //
            App.Player.PlaybackSession.EffectContainer.EqualizerController?.SetEqPreset(preset);
            WindowTheme.Refresh();
        }

        public static void SaveAppConfigs()
        {
            try
            {
                CurrentConfig = new()
                {
                    LastFile = AppStatics.LastFile,
                    IsDark = AppStatics.IsDark,
                    WindowsWidth = AppStatics.WindowsWidth,
                    WindowsHeight = AppStatics.WindowsHeight,
                    WindowsLeft = AppStatics.WindowsLeft,
                    WindowsTop = AppStatics.WindowsTop,
                    IsWindowStateMaximized = AppStatics.IsWindowStateMaximized,
                    IsEqualizerEnabled = App.Player.PlaybackSession.EffectContainer.EnableEqualizer,
                    IsPitchShiftingEnabled = App.Player.PlaybackSession.EffectContainer.EnablePtchShifting,
                    AudioVolume = (double)App.Player.PlaybackSession.VolumeController.Volume
                };
                if (App.Player.PlaybackSession.EffectContainer.EqualizerController != null)
                {
                    CurrentConfig.EqBandsGain = App.Player.PlaybackSession.EffectContainer.EqualizerController?.AllBandsGain;
                }
                else
                {
                    CurrentConfig.EqBandsGain = Array.Empty<int>();
                }
                WriteConfig();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

    }
}