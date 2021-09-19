using Engine;
using Engine.Enums;
using MusicApplication.ViewModel.Base;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        private async void TimeProgressBar_MouseMove(object sender, MouseEventArgs e)
        {

            e.Handled = true;
            ProgressBar progressBar = (ProgressBar)sender;
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {

                double val = SetPbValue(e.GetPosition(progressBar).X, progressBar);
                if (val != Player.CurrentTime.TotalSeconds
                    && val <= Player.TotalTime.TotalSeconds)
                {

                    //progressBar.Value = val;
                    //progressBar.GetBindingExpression(ProgressBar.ValueProperty).UpdateSource();
                    await Player.SeekAsync(val);
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
                Player.ChangeVolume(SetPbValue(e.GetPosition(progressBar).X, progressBar));
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

        public PlayerControlViewModel()
        {
            PlayPauseCommand = new DelegateCommand(() => PlayPause());

            Player.PlaybackStateChanged += PlaybackStateChanged;
            Player.CurrentTimeChanged += AudioPlayer_CurrentTimeChanged;
            Player.VolumeChanged += AudioPlayer_VolumeChanged;
            //PlaylistManager.PlaylistCurrentFileChanged += PlaylistManager_PlaylistCurrentFileChanged;
        }

        private async void PlaylistManager_PlaylistCurrentFileChanged()
        {
            //bool tb = !PlaylistManager.IsFirst(PlaylistManager.Playlists[0], MainCommands.Source);
            await Task.Run(() => previousAudioFileCommand.IsEnabled = false);
        }

        private void PlayPause()
        {
            if (IsPlaying)
            {
                Player.Pause();
            }
            else if (Player.PlaybackState is PlaybackState.Paused
                    or PlaybackState.Stopped
                    or PlaybackState.Ended)
            {
                Player.Play();
            }
            else
            {
                Shared.OpenFilePicker();
            }
        }

        private async void AudioPlayer_VolumeChanged()
        {
            await Task.Run(() => { NotifyPropertyChanged(nameof(Volume)); });
        }

        private async void AudioPlayer_CurrentTimeChanged(TimeSpan Time)
        {
            await Task.Run(() =>
            {
                crtts = Time.TotalSeconds;
                NotifyPropertyChanged(nameof(CurrentTimeTotalSeconds));
                NotifyPropertyChanged(nameof(CurrentTimeString));
            });
        }

        private async void PlaybackStateChanged(PlaybackState newPlaybackState)
        {
            await Task.Run(() =>
            {
                if (newPlaybackState == PlaybackState.Playing)
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
            });
        }
        public string CurrentTimeString => Player.CurrentTime.ToString(Shared.stringformat);

        public string TotalTimeString => Player.TotalTime.ToString(Shared.stringformat);

        private double crtts;
        public double CurrentTimeTotalSeconds
        {
            get => crtts;//Player.CurrentSeconds;
            set
            {
                crtts = value;
                //Task.Run(async () => { await Player.SeekAsync(value); });
            }
        }

        public double TotalTimeTotalSeconds => Player.TotalTime.TotalSeconds;

        public double Volume
        {
            get => Player.Volume;
            set => Player.Volume = value;
        }

        public bool IsPlaying { get; set; }

        private DelegateCommand nextAudioCommand;
        public ICommand NextAudioCommand => nextAudioCommand ??= new DelegateCommand(NextAudio);

        private void NextAudio()
        {
            PlaylistManager.PlayNext();
        }

        private DelegateCommand previousAudioFileCommand;
        public ICommand PreviousAudioFileCommand => previousAudioFileCommand ??= new DelegateCommand(PreviousAudioFile);

        private void PreviousAudioFile()
        {
            PlaylistManager.PlayPrevious();
        }
    }

}
