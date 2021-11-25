using NAudio.Wave;

namespace PlayerLibrary.Core
{
    public class SampleProviderBridge
    {
        public ISampleProvider InputSampleProvider { get; internal set; }

        public ISampleProvider OutputSampleProvider { get; set; }

    }
}