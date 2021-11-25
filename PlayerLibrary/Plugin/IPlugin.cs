using NAudio.Wave;

namespace PlayerLibrary.Plugin
{
    public interface IPlugin
    {
        public void Enable();

        public void Disable();

        public bool IsEnabled { get; }

        public ISampleProvider InputSampleProvider { get; set; }

        public ISampleProvider OutputSampleProvider { get; }
    }
}