using PlayerLibrary;
using System.Runtime.InteropServices;

namespace TestWinForm
{
    public partial class Form1 : Form
    {
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            Helper.DarkUi.DwmApi.ToggleImmersiveDarkMode(Handle,true);
        }
        public Form1()
        {
            InitializeComponent();
            Player = new Player();
            Player.PlaybackSession.TimelineController.TimePositionChanged += TimelineController_TimePositionChanged;
        }

        readonly Player Player;
        private void button1_Click(object sender, EventArgs e)
        {
            Player.PlaybackSession.Open(new Uri(@"C:\Users\ahmad\Downloads\Music\Ebrahim Al Baghdadi - Vol Volek_(MadarMusic.ir).mp3"));
            Player.PlaybackSession.Play();
            trackBar1.Maximum = (int)Player.PlaybackSession.TimelineController.Total.TotalSeconds;

        }

        private void TimelineController_TimePositionChanged(TimeSpan timeSpan)
        {
            trackBar1.Value = (int)timeSpan.TotalSeconds;
        }

        private void trackBar1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Player.PlaybackSession.TimelineController.Seek(TimeSpan.FromSeconds(trackBar1.Value));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Player.PlaybackSession.EffectContainer.EnablePtchShifting= !Player.PlaybackSession.EffectContainer.EnablePtchShifting;
        }
    }
}