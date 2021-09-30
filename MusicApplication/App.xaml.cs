using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace MusicApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        [DllImport("Kernel32")]
        private static extern void AllocConsole();

        protected override void OnStartup(StartupEventArgs e)
        {
            AllocConsole();
            Debug.WriteLine($"AppOnStartUp-args:[{ e.Args}]");
            base.OnStartup(e);
            WindowsManager.StartApp(e.Args);
        }
    }
}
