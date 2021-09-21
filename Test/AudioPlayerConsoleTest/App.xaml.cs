using Engine;
using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Test
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AllocConsole();
            Console.WriteLine("Started");
            Player.Initialize();
            PlaylistManager.Initialize();
            Player.Source = e.Args[0];
            await Player.OpenAsync();

            WAitForCmd();
        }

        private void WriteLine(object obj)
        {
            Console.WriteLine(obj);
        }

        private async void WAitForCmd()
        {
            WriteLine(null);
            switch (Console.ReadLine())
            {
                case "info":
                    WriteLine("playback state: " + Player.PlaybackState);
                    WriteLine("source: " + Player.Source);
                    WriteLine($"Time Seconds: {Player.CurrentTime.TotalSeconds} / {Player.TotalTime.TotalSeconds}");
                    WriteLine($"Time Seconds: {Player.CurrentTime.ToString()} / {Player.TotalTime.ToString()}");
                    WriteLine("volume: " + Player.Volume);
                    break;
                case "play":
                    Player.Play();
                    break;
                case "pause":
                    Player.Pause();
                    break;
                case "stop":
                    Player.Stop();
                    break;
                case "open":
                    await Player.OpenAsync();
                    break;
                case "close":
                    Player.Close();
                    break;
                case "v down":
                    Player.VolumeDown(10);
                    break;
                case "v up":
                    Player.VolumeUp(10);
                    break;
                case "seek end":
                    await Player.SeekAsync(Player.TotalTime.TotalSeconds - 10);
                    break;
                case "clear":
                    Console.Clear();
                    break;
                default:
                    Console.WriteLine("Not Found");
                    break;
            }
            WAitForCmd();
        }

        private static string[] cmdList = { "", "", "" };

    }
}
