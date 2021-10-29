using PlayerLibrary;
using PlayerUI.Commands;
using PlayerUI.Config;
using System;
using System.Windows;

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
            try
            {
                base.OnStartup(e);
                Commands.App.LoadStartupConfigs();
                MainWindow.Show();
                Commands.App.LoadStartupArgs(e.Args);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Microsoft.Win32.SystemEvents.UserPreferenceChanged += (_, _) =>
            {
                AppStatics.IsDark = Helper.ThemeListener.RegistryisDark();
                WindowTheme.Refresh();
            };
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Commands.App.SaveStartupConf();
        }
    }
}
