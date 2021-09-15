using Engine.Commands;
using Engine.Enums;
using Engine.Events.Base;
using Engine.Internal;
using Engine.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Engine
{
    public static class PlaylistManager
    {

        public static event EventHandlerNull PlaylistCurrentFileChanged;
        public static int OpenedFileIndex { get; set; }

        private static void FindFileInPlaylist(string file)
        {
            int i = Playlists[0].FindItemlambda(file);
            if (i != -1)
            {
                OpenedFileIndex = i;
                PlaylistCurrentFileChanged?.Invoke();
            };
        }

        public static void FindOpenedFileIndex()
        {
            int i = Playlists[0].FindItemlambda(Player.Source);
            if (i != -1)
            {
                OpenedFileIndex = i;
            };
            _ = (PlaylistCurrentFileChanged?.Invoke());
        }

        #region Playlist
        //
        public static void Initialize()
        {
            Playlists.Add(new PlaylistFile());
        }
        public static List<PlaylistFile> Playlists { get; set; } = new();

        public static List<AudioFile> PlaylistItems { get => Playlists[0].Items; }

        public static void Add(int PlaylistIndex, string File)
        {
            Playlists[PlaylistIndex].AddItem(File);
        }

        public static async Task AddRangeAsync(int PlaylistIndex, string[] Files)
        {
            await Playlists[PlaylistIndex].AddRangeAsync(Files);
        }

        public static void Remove(int PlaylistIndex, int index)
        {
            Playlists[PlaylistIndex].Items.RemoveAt(index);
        }

        public static void Clear()
        {
            Playlists[0].Clear();
            GC.Collect();
        }

        internal static void PlaybackEnded()
        {
            Task.Run(() =>
           {
               Task.Delay(1000).Wait();
           });
            Player.Pause();
            Player.Seek(0);
            Player.Play();
            return;
        }

        #endregion

        private static bool IsFirst(PlaylistFile playlist, string OpenedItem)
        {
            return playlist.FindItemlambda(OpenedItem) == 0;
        }

        private static bool IsLast(PlaylistFile playlist, string OpenedItem)
        {
            return playlist.FindItemlambda(OpenedItem) == playlist.Items.Count - 1;
        }

        public static void PlayNext()
        {
            if (!IsLast(Playlists[0], Player.Source))
            {
                MainCommands.Source = Playlists[0].Items[OpenedFileIndex + 1].FilePath;
            }
        }

        public static void PlayPrevious()
        {
            if (!IsFirst(Playlists[0], Player.Source))
            {
                MainCommands.Source = Playlists[0].Items[OpenedFileIndex - 1].FilePath;
            }

        }

        internal static RepeatMode RepeatMode { get; set; } = RepeatMode.CurrentFile;
        internal static async Task RepeaterAsync()
        {
            await Task.Run(() =>
            {
                if (true)
                {
                    switch (RepeatMode)
                    {
                        case RepeatMode.Stop:
                            MainCommands.Stop();
                            break;
                        case RepeatMode.Close:
                            MainCommands.Close();
                            break;
                        case RepeatMode.CurrentFile:
                            MainCommands.Stop();
                            _ = Player.SeekAsync(0);
                            MainCommands.Play();
                            break;
                        case RepeatMode.NextFile:
                            break;
                        case RepeatMode.CurrentPlaylist:
                            break;
                        default:
                            break;
                    }
                }
            });
        }
    }
}
