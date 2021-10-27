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
            SettingLoader.LoadAppSettings();
            MainWindow.Show();
            try
            {
                _ = Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async () =>
                  {
                      await Task.Run(async () => await LoadArgAsync(e.Args));
                  }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
            Microsoft.Win32.SystemEvents.UserPreferenceChanged += (_, _) =>
            {
                Statics.IsDark = Helper.ThemeListener.RegistryisDark();
                Commands.WindowTheme.Refresh();
            };
        }

        private static async Task LoadArgAsync(string[] args)
        {
            await Task.Run(async () =>
             {
                 if (args?.Length > 0)
                 {
                     await Player.OpenAsync(args[0]);
                 }
                 else
                 {
                     string file = Settings.CurrentConfig.LastFile;
                     if (!string.IsNullOrEmpty(file))
                     {
                         await Player.OpenAsync(file);
                     };
                 }
             });
        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            SettingLoader.SaveAppSettings();
        }
    }
}
