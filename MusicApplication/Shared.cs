using Helper.Utility;
using Engine.Commands;

namespace MusicApplication
{
    public static class Statics
    {
        public static void OpenCurrentFileLocation()
        {
            OpenFileLocation.Open(MainCommands.Source);
        }
    }

}
