namespace PlayerUI.Common.Config
{
    public record ConfigModel
    {
        [JsonConstructor]
        public ConfigModel()
        {
        }

        public string LastFile { get; set; } = string.Empty;
        public bool IsDark { get; set; } = false;
        public double WindowsWidth { get; set; }
        public double WindowsHeight { get; set; }
        public double WindowsLeft { get; set; }
        public double WindowsTop { get; set; }
        public bool IsWindowStateMaximized { get; set; }
        public int[] EqBandsGain { get; set; }
    }
}
