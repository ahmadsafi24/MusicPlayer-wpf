using Helper.ViewModelBase;
using System.Windows.Input;
using static NAudio.Wave.MediaFoundationReader;

namespace Test
{
    public class ViewModel : ViewModelBase
    {
        public ICommand TestCommand1 { get; }
        public ICommand TestCommand2 { get; }

        public ViewModel()
        {
            TestCommand1 = new DelegateCommand(Test1);
            TestCommand2 = new DelegateCommand(Test2);
        }

        private void Test2()
        {
            player = new();
            player.PlaybackStateChanged += Player_PlaybackStateChanged;
            player.Open(filepath);
            player.Play();
            player.ChangeVolume(100);
        }

        private void Player_PlaybackStateChanged(PlayerLibrary.PlaybackState playbackState)
        {
            log += playbackState.ToString() + "\n";
            NotifyPropertyChanged(nameof(log));
        }

        public string? log { get; set; }
        PlayerLibrary.Player? player;



        private void Test1()
        {
            //
            //MediaCommands.Play
            MediaFoundationReaderSettings readerSettings = new() { RequestFloatOutput = false, RepositionInRead = false };
            //
            WaveOutEvent?.Dispose();
            Reader?.Dispose();

            Reader = new(filepath, readerSettings);
            WaveOutEvent = new();

            WaveOutEvent?.Init(Reader);
            WaveOutEvent?.Play();
        }

        static string filepath = @"C:\Users\ahmad\Downloads\Telegram Desktop\247-Pro_20_M3822P_V1.0_HEVC_V3.8_211011.abs";
        static NAudio.Wave.MediaFoundationReader? Reader;
        static NAudio.Wave.WaveOutEvent? WaveOutEvent;
    }
}
