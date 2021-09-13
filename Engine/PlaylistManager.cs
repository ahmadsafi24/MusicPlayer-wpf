using Engine.Events.Base;
using Engine.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Engine
{
    public static class PlaylistManager
    {
        public static void Initialize()
        {
            Playlist.Add(new PlaylistFile());
        }
        public static int PlaylistCurrentFileIndex { get; set; }
        public static List<PlaylistFile> Playlist { get; set; } = new();
        public static event EventHandlerNull PlaylistCurrentFileChanged;

        public static AudioFile CurrentFileTag { get; set; }
        public static List<AudioFile> PlaylistItems { get => Playlist[0].FileList; }

        private static void FindFileInPlaylist(string file)
        {
            int i = Playlist[0].FindItem(file);
            if (i != -1)
            {
                PlaylistCurrentFileIndex = i;
                PlaylistCurrentFileChanged?.Invoke();
            };
        }

        public static void FindCurrentFile()
        {
            int i = Playlist[0].FindItem(CurrentFileTag?.FilePath);
            if (i != -1)
            {
                PlaylistCurrentFileIndex = i;
            };
            _ = (PlaylistCurrentFileChanged?.Invoke());
        }

        //

        public static void Add(int PlaylistIndex, string File)
        {
            Playlist[PlaylistIndex].AddItem(File);
        }

        public static async Task AddRangeAsync(int PlaylistIndex, string[] Files)
        {
            await Task.Run(() =>
            {
                Playlist[PlaylistIndex].AddRange(Files);
            });
        }

        public static void Remove(int PlaylistIndex, int index)
        {
            Playlist[PlaylistIndex].FileList.RemoveAt(index);
        }

        public static void Clear()
        {
            Playlist[0].Clear();
        }
        //
    }
}
