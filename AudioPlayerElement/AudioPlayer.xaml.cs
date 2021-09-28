using Engine;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AudioPlayerElement
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AudioPlayer : UserControl
    {
        Player Player = new();
        public AudioPlayer()
        {
            InitializeComponent();
            Player.Source = @"D:\\temp\\music.mp3";
            Task.Run(async () => await Player.OpenAsync());
            Player.CurrentTimeChanged += Player_CurrentTimeChanged;
        }

        private void Player_CurrentTimeChanged(System.TimeSpan Time)
        {
            this.CaptionString = Time.ToString();
        }

        public string CaptionString
        {
            get => GetValue(CurrentTimeStringProperty) as string;
            set => SetValue(CurrentTimeStringProperty, value);
        }
        public static readonly DependencyProperty CurrentTimeStringProperty = DependencyProperty.Register(
          "CurrentTimeString", typeof(string), typeof(AudioPlayer), new PropertyMetadata("CurrentTimeString"));
    }
}