namespace PlayerLibrary.Model
{
    public class EqPreset
    {
        public EqPreset(string equalizerMode, int[] bandsGain)
        {
            EqualizerMode = equalizerMode;
            BandsGain = bandsGain;
        }

        public string EqualizerMode { get; set; }
        public int[] BandsGain { get; set; }
    }
}