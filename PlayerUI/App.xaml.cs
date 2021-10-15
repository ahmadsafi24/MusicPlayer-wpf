using PlayerLibrary;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace PlayerUI
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

            //Window ResourceManager = new Windows.ResourceManager();
            //ResourceManager.Show();

            await Task.Run(async () =>
            {
                if (e.Args?.Length > 0)
                {
                    _ = Task.Run(async () => await Player.Playlist.AddRangeAsync(e.Args));
                    await Player.OpenAsync(e.Args[0]);
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
