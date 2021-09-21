using Engine;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace MusicApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        protected override void OnStartup(StartupEventArgs e)
        {
            AllocConsole();
            Debug.WriteLine($"AppOnStartUp-args:[{ e.Args}]");
            base.OnStartup(e);
            Player.Initialize();
            PlaylistManager.Initialize();
            WindowsManager.StartApp(e.Args);
        }
    }
}
