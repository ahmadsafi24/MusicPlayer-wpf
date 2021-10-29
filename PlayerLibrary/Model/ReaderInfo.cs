using NAudio.Wave;

namespace PlayerLibrary.Model
{
    public class ReaderInfo
    {
        public int SampleRate { get; private set; }
        public int Channels { get; private set; }
        public int AverageBytesPerSecond { get; private set; }
        public WaveFormatEncoding Encoding { get; private set; }
        public int BitsPerSample { get; private set; }

        public ReaderInfo(MediaFoundationReader reader)
        {
            SampleRate = reader.WaveFormat.SampleRate;
            Channels = reader.WaveFormat.Channels;
            AverageBytesPerSecond = reader.WaveFormat.AverageBytesPerSecond;
            Encoding = reader.WaveFormat.Encoding;
            BitsPerSample = reader.WaveFormat.BitsPerSample;
        }

        public override string ToString()
        {
            _ = base.ToString();
            return $"Media Info <SampleRate: {SampleRate}> | <Channels: {Channels}> | <avgBPM {AverageBytesPerSecond}> | <Encoding: {Encoding}> | <BPM: {BitsPerSample}>";
        }
    }

}
