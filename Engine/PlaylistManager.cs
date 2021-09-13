using Engine.Events.Base;
using Engine.Model;

namespace Engine
{
    public static class PlaylistManager
    {
        public static int PlaylistCurrentFileIndex { get; set; }
        public static PlaylistFile Playlist { get; } = new();
        public static event EventHandlerNull PlaylistCurrentFileChanged;

        public static AudioFile CurrentFileTag { get; set; }

        private static void FindFileInPlaylist(string file)
        {
            int i = Playlist.FindItem(file);
            if (i != -1)
            {
                PlaylistCurrentFileIndex = i;
                PlaylistCurrentFileChanged?.Invoke();
            };
        }

        public static void FindCurrentFile()
        {
            int i = Playlist.FindItem(CurrentFileTag?.FilePath);
            if (i != -1)
            {
                PlaylistCurrentFileIndex = i;
            };
            _ = (PlaylistCurrentFileChanged?.Invoke());
        }

        //

        public static void Add(string File)
        {
            Playlist.AddItem(File);
        }

        public static void Remove(int index)
        {
            Playlist.FileList.RemoveAt(index);
        }
        //
    }
}
