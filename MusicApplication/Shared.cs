using Engine.Commands;
using System.Configuration;

namespace MusicApplication
{
    public static class Statics
    {
        public static void OpenCurrentFileLocation()
        {
            Helper.OpenFileLocation.Open(MainCommands.Source);
        }
        public static void test()
        {
            _ = ConfigurationManager.ConnectionStrings.CurrentConfiguration.FilePath;
        }
    }

}
