using System.Text.Json.Serialization;

namespace PlayerLibrary.Model
{
    public record EqPreset
    {
        [JsonConstructor]
        public EqPreset(string equalizerMode, int[] bandsGain)
        {
            EqualizerMode = equalizerMode;
            BandsGain = bandsGain;
        }

        public string EqualizerMode { get; set; }
        public int[] BandsGain { get; set; }
    }
}