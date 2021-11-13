using System.Windows;

namespace Test
{
    public partial class MainWindow : Window
    {
        readonly PlayerLibrary.Player Player = new();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            Player.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
            Player.PlaybackSession.TimelineController.TimePositionChanged += TimelineController_TimePositionChanged;
            Player.PlaybackSession.Open(txt1.Text);
            Player.PlaybackSession.Play();
        }

        private void TimelineController_TimePositionChanged(System.TimeSpan timeSpan)
        {
            txblck1.Text = PlayerLibrary.Converter.TimeSpanToString.FromTimespan(timeSpan);
        }

        private void PlaybackSession_PlaybackStateChanged(PlayerLibrary.PlaybackState playbackState)
        {
            //throw new System.NotImplementedException();
        }
    }
}
