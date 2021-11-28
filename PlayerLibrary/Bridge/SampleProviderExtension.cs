using NAudio.Wave;
using PlayerLibrary.Core;

namespace PlayerLibrary.Bridge
{
    public static class SampleProviderExtension
    {
        public static VolumeController CreateVolumeController(this ISampleProvider input)
        {
            VolumeController output = new(input);
            return output;
        }
    }
}