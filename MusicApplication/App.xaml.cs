using AudioPlayer;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace MusicApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static Player Player = new();

        protected override async void OnStartup(StartupEventArgs e)
        {
            Debug.WriteLine($"AppOnStartUp-args:[{ e.Args}]");
            base.OnStartup(e);

            Setting.Load.LoadIsDark();

            MainWindow.Show();
            MainWindow.Content = View.AllViews.MainView;

            await Task.Run(async () =>
            {
                if (e.Args?.Length > 0)
                {
                    _ = Task.Run(async () => await Player.Playlist.AddRangeAsync(e.Args));
                    Player.Source = e.Args[0];
                    await Player.OpenAsync();
                }
            });

        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Setting.Save.SaveIsDark();
        }
    }
}
