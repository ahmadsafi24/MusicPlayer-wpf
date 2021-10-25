using PlayerLibrary;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PlayerUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static Player Player = new();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                Setting.Load.LoadIsDark();

                MainWindow.Show();
                Task.Run(async () =>
                {
                    if (e.Args?.Length > 0)
                    {
                        await Player.OpenAsync(e.Args[0]);
                    }
                });
            }));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Setting.Save.SaveIsDark();
        }
    }
}
