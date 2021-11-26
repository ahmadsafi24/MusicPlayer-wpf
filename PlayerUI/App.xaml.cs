using Microsoft.Windows.ApplicationModel.DynamicDependency;

namespace PlayerUI
{
    public partial class App : Application
    {
        //[System.Runtime.InteropServices.DllImport("Kernel32")]
        //private static extern void AllocConsole();

        internal static Player Player = new();

        protected override void OnStartup(StartupEventArgs e)
        {
            //AllocConsole();

            base.OnStartup(e);

            try
            {

                AppCommands.LoadStartupConfigs();
                MainWindow.Show();
                AppCommands.LoadStartupArgs(e.Args);
                EnableSystemThemeListener();
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
            if (e.ApplicationExitCode != 0)
            { MessageBox.Show(e.ApplicationExitCode.ToString()); }

            Common.Commands.AppCommands.SaveStartupConf();
        }

        static void EnableSystemThemeListener()
        {
            Microsoft.Win32.SystemEvents.UserPreferenceChanged += (_, _) =>
            {
                AppStatics.IsDark = ThemeListener.RegistryisDark();
                WindowTheme.Refresh();
            };
        }
    }
}
