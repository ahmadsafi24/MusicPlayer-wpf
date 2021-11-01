namespace PlayerLibrary.Model
{
    public class EqPreset
    {
        public EqPreset(EqualizerMode equalizerMode, int[] bandsGain)
        {
            EqualizerMode = equalizerMode;
            BandsGain = bandsGain;
        }

        public EqualizerMode EqualizerMode { get; set; }
        public int[] BandsGain { get; set; }
    }
}