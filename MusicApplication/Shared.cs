using Helper.Utility;
using Engine.Commands;
using System.Configuration;
using System;

namespace MusicApplication
{
    public static class Statics
    {
        public static void OpenCurrentFileLocation()
        {
            OpenFileLocation.Open(MainCommands.Source);
        }
        public static void test()
        {
            _ = ConfigurationManager.ConnectionStrings.CurrentConfiguration.FilePath;
        }
    }

}
