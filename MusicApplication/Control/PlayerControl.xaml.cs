using Engine;
using MusicApplication.ViewModel.Base;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Engine.Commands;

namespace MusicApplication.Control
{
    /// <summary>
    /// Interaction logic for PlayerControl.xaml
    /// </summary>
    public partial class PlayerControl : UserControl
    {
        public PlayerControl()
        {
            SizeChanged += PlayerControl_SizeChanged;
            InitializeComponent();
        }

        private void PlayerControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TimeProgressBar_ValueChanged(TimeProgressbar, null);
            VolumeProgressbar_ValueChanged(VolumeProgressbar, null);
        }

        private void TimeProgressBar_MouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            ProgressBar progressBar = (ProgressBar)sender;
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                double val = SetPbValue(e.GetPosition(progressBar).X, progressBar);
                if (MainCommands.TotalSeconds != val
                    && val <= MainCommands.TotalSeconds)
                {
                    MainCommands.Seek(val);
                }
            }
        }

        private void TimeProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ProgressBar progressBar = (ProgressBar)sender;
            switch (progressBar.Maximum)
            {
                case 0:
                    EllipseThumb.Visibility = Visibility.Hidden;
                    return;
                default:
                    EllipseThumb.Visibility = Visibility.Visible;
                    break;
            }
            EllipseThumb.Margin = SetEllipseMargin(progressBar, EllipseThumb.ActualWidth);
        }

        private void VolumeProgressbar_MouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            ProgressBar progressBar = (ProgressBar)sender;
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                MainCommands.ChangeVolume(SetPbValue(e.GetPosition(progressBar).X, progressBar));
            }
        }

        private void VolumeProgressbar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (EllipseThumbVol != null)
            {
                EllipseThumbVol.Margin = SetEllipseMargin(VolumeProgressbar, EllipseThumbVol.ActualWidth);
            }
        }


        private void ProgressBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _ = Mouse.Capture((ProgressBar)sender);
            e.Handled = true;
        }

        private void ProgressBar_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _ = Mouse.Capture(null);
            e.Handled = true;

        }


        private static Thickness SetEllipseMargin(ProgressBar progressBar, double ellipseWidth)
        {
            double ratio = progressBar.Maximum / progressBar.Value;
            if (double.IsNaN(ratio) == false)
            {
                double leftpos = progressBar.ActualWidth / ratio;
                double val = leftpos + (ellipseWidth / 4);
                return new Thickness(val, 0, 0, 0);
            }

            return new Thickness(0);
        }

        private static double SetPbValue(double mousePosition, ProgressBar progressBar)
        {
            double ratio = mousePosition / progressBar.ActualWidth;
            double pbvalue = ratio * progressBar.Maximum;
            return pbvalue;
        }

    }

    public class PlayerControlViewModel : ViewModelBase
    {
        public ICommand PlayPauseCommand { get; }
        public ICommand OpenFileCommand { get; }

        public PlayerControlViewModel()
        {
            PlayPauseCommand = new DelegateCommand(() => MainCommands.PlayPause());
            OpenFileCommand = new DelegateCommand(() => MainCommands.OpenFilePicker());

            Engine.Events.AllEvents.PlaybackStateChanged += PlaybackStateChanged;
            Engine.Events.AllEvents.CurrentTimeChanged += AudioPlayer_CurrentTimeChanged;
            Engine.Events.AllEvents.VolumeChanged += AudioPlayer_VolumeChanged;
        }

        private async Task AudioPlayer_VolumeChanged()
        {
            await Task.Run(() => { NotifyPropertyChanged(nameof(Volume)); });
        }

        private async Task AudioPlayer_CurrentTimeChanged(TimeSpan Time)
        {
            await Task.Run(() =>
            {
                NotifyPropertyChanged(nameof(CurrentTimeString));
                NotifyPropertyChanged(nameof(CurrentTimeTotalSeconds));
            });
        }

        private async Task PlaybackStateChanged(Engine.Enums.PlaybackState newPlaybackState)
        {
            await Task.Delay(0);
            if (newPlaybackState == Engine.Enums.PlaybackState.Playing)
            {
                IsPlaying = true;
                NotifyPropertyChanged(nameof(IsPlaying));
            }
            else
            {
                IsPlaying = false;
                NotifyPropertyChanged(nameof(IsPlaying));
            }

            NotifyPropertyChanged(nameof(TotalTimeString));
            NotifyPropertyChanged(nameof(CurrentTimeString));
            NotifyPropertyChanged(nameof(TotalTimeTotalSeconds));
            NotifyPropertyChanged(nameof(CurrentTimeTotalSeconds));
        }

        public string CurrentTimeString => MainCommands.CurrentTimeString;

        public string TotalTimeString => MainCommands.TotalTimeString;

        public double CurrentTimeTotalSeconds
        {
            get => MainCommands.CurrentSeconds;
            set => MainCommands.Seek(value);
        }

        public double TotalTimeTotalSeconds => MainCommands.TotalSeconds;

        public double Volume
        {
            get => MainCommands.Volume;
            set => MainCommands.Volume = value;
        }

        public bool IsPlaying { get; set; }

        private DelegateCommand nextAudioCommand;
        public ICommand NextAudioCommand => nextAudioCommand ??= new DelegateCommand(NextAudio);

        private void NextAudio()
        {
            ItemPlayManager.NextAudio();
        }

        private DelegateCommand previousAudioFileCommand;
        public ICommand PreviousAudioFileCommand => previousAudioFileCommand ??= new DelegateCommand(PreviousAudioFile);

        private void PreviousAudioFile()
        {
            ItemPlayManager.PreviousAudioFile();
        }
    }

}
