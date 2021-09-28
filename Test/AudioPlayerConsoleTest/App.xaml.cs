using Engine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;

namespace Test
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Player Player = new();

        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AllocConsole();
            WriteLine("Started");
            //PlaylistManager.Initialize();
            Player.Source = e.Args[0];
            await Player.OpenAsync();

            WAitForCmd();
        }

        private static void WriteLine(object obj)
        {
            Console.WriteLine(obj);
        }

        private void WAitForCmd()
        {
            WriteLine(null);

            /*switch (Console.ReadLine())
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
                    WriteLine("Not Found");
                    break;
            }*/

            string cmdstr = Console.ReadLine();


            int i = Actions.FindIndex(x => x.Method.Name.ToString() == cmdstr);
            if (i >= 0)
            {
                Actions[i].Invoke();
                WAitForCmd();
            }

            string trimmedstr = cmdstr.TrimStart('"');
            trimmedstr = trimmedstr.TrimEnd('"');
            if (System.IO.File.Exists(trimmedstr))
            {
                Player.Source = trimmedstr;

                open();
            }
            else
            {
                WriteLine($"Command: <{cmdstr}> Not Found");
                WriteLine("Available Commands: ");
                help();
            }
            WAitForCmd();
        }

        private static readonly List<Action> Actions = new()
        {
            new Action(open),
            new Action(play),
            new Action(pause),
            new Action(help)
        };

        private static void help()
        {
            foreach (var item in Actions)
            {
                WriteLine(item.Method.Name);
            }
        }

        private static async void open()
        {
            await Player.OpenAsync();
        }

        private static void pause()
        {
            Player.Pause();
        }

        private static void play()
        {
            Player.Play();
        }
    }
}
