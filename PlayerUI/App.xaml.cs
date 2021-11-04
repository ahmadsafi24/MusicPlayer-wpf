using PlayerLibrary;
using PlayerUI.Commands;
using PlayerUI.Config;
using System;
using System.Windows;

namespace PlayerUI
{
    public partial class App : Application
    {
        [System.Runtime.InteropServices.DllImport("Kernel32")]
        public static extern void AllocConsole();

        internal static Player Player = new();

        protected override void OnStartup(StartupEventArgs e)
        {
            AllocConsole();

                base.OnStartup(e);
                Commands.App.LoadStartupConfigs();
                MainWindow.Show();
                Commands.App.LoadStartupArgs(e.Args);
            EnableSystemThemeWatcher();
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n{ex.StackTrace}", "OnAppStart");
                throw;
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Commands.App.SaveStartupConf();
        }

        void EnableSystemThemeWatcher()
        {
            Microsoft.Win32.SystemEvents.UserPreferenceChanged += (_, _) =>
            {
                AppStatics.IsDark = Helper.ThemeListener.RegistryisDark();
                WindowTheme.Refresh();
            };
        }
    }
}
