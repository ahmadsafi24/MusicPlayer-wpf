using Helper;
using NAudio.Wave;
using System.Threading.Tasks;
using PlayerLibrary.Model;
using System;

namespace PlayerLibrary.FileInfo
{
    public class AudioInfo
    {
        public int SampleRate { get; set; }

        public int Channels { get; set; }

        public int AverageBytesPerSecond { get; set; }

        public string Format { get; set; }

        public int BitsPerSample { get; set; }

        public string BitrateString { get; set; }

        public readonly string Bitrate;

        public AudioTag AudioTag { get; set; }

        public AudioInfo(Uri filePath)
        {
            try
            {
                if (filePath == null) return;
                AudioTag = new(filePath.OriginalString);

                if (string.IsNullOrEmpty(filePath.AbsolutePath))
                {
                    // Task.Run(async () => await Log.ShowMessage("empty in audioinfo"));
                    return;
                }
                else
                {

                    Mp3FileReader fileReader = new(filePath.OriginalString);
                Mp3WaveFormat mp3WaveFormat = fileReader.Mp3WaveFormat;

                SampleRate = mp3WaveFormat.SampleRate;
                Channels = mp3WaveFormat.Channels;
                AverageBytesPerSecond = mp3WaveFormat.AverageBytesPerSecond;
                if (mp3WaveFormat.Encoding.ToString() == "MpegLayer3")
                {
                    Format = "MP3";
                }
                else
                {
                    Format = mp3WaveFormat.Encoding.ToString();
                }
                BitsPerSample = mp3WaveFormat.BitsPerSample;

                Mp3Frame frame = fileReader?.ReadNextFrame();
                frame = fileReader?.ReadNextFrame();
                int bitrate = (int)(frame?.BitRate);

                string bitrateString = null;
                if (bitrate > 1000)
                { bitrateString = $"{bitrate /= 1000}kbps"; }
                else
                { bitrateString = $"{bitrate}bps"; }

                BitrateString = bitrateString;
            }

            }
            catch (System.Exception ex)
            {
                Log.WriteLine("AudioInfo",ex.Message);
            }
        }


    }
}
