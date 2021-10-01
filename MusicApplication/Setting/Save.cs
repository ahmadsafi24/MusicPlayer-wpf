using System.IO;

namespace MusicApplication.Setting
{
    public static class Save
    {
        public static void SaveIsDark()
        {
            Directory.CreateDirectory(@"Setting\");
            File.WriteAllText(@"Setting\IsDark", Theme.WindowTheme.IsDark.ToString());
        }
    }
}
