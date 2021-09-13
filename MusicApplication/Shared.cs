using System.Windows;
using Helper.Utility;
using Engine.Commands;
using Engine.Events;

namespace MusicApplication
{
    public static class Statics
    {
        public static void OpenCurrentFileLocation()
        {
            OpenFileLocation.Open(MainCommands.Source);
        }
        //public static string TimeSpanToStringFormat { get; } = "mm\\:ss";


    }

}
