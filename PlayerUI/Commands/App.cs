namespace PlayerUI.Commands
{
    public static class App
    {
        public static void CloseMainWindow()
        {
            System.Windows.Application.Current.MainWindow.Close();
        }

        public static void ShutDown()
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
