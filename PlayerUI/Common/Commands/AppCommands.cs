namespace PlayerUI.Common.Commands
{
    public static class AppCommands
    {
        public static void CloseMainWindow()
        {
            System.Windows.Application.Current.MainWindow.Close();
        }

        public static void ShutDown()
        {
            System.Windows.Application.Current.Shutdown();
        }

        public static void LoadStartupConfigs()
        {
            AppConfig.LoadConfigs();
        }

        public static void SaveStartupConf()
        {
            AppConfig.SaveAppConfigs();
        }

        public static async void LoadStartupArgs(string[] args)
        {
            await PlayerUI.App.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new System.Action(async () =>
           {
               await System.Threading.Tasks.Task.Run(async () =>
               {
                   if (args?.Length > 0)
                   {
                       await PlayerUI.App.Player.PlaybackSession.OpenAsync(args[0]);
                   }
                   else
                   {
                       string file = AppConfig.CurrentConfig.LastFile;
                       if (!string.IsNullOrEmpty(file))
                       {
                           await PlayerUI.App.Player.PlaybackSession.OpenAsync(file);
                       };
                   }
               });
           }));
        }
    }
}
