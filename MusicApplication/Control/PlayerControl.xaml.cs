using System;
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
        private readonly AudioPlayer.Player Player = App.Player;
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
                if (val != Player.TimePosition.TotalSeconds
                    && val <= Player.TimeDuration.TotalSeconds)
                {
                    await Player.SeekAsync(val);
                }
            }
        }

        private void TimeProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void VolumeProgressbar_MouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            ProgressBar progressBar = (ProgressBar)sender;
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Player.ChangeVolume((int)SetPbValue(e.GetPosition(progressBar).X, progressBar));
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

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
