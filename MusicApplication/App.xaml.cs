using Engine;
using Engine.Commands;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MusicApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            Debug.WriteLine($"AppOnStartUp-args:[{ e.Args}]");
            base.OnStartup(e);
            MainCommands.Initialize();
            PlaylistManager.Initialize();
            WindowsManager.StartApp(e.Args);
        }
    }
}
