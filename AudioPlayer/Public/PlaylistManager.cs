using Engine.Enums;
using Engine.Internal;
using Engine.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Engine
{
    public static class PlaylistManager
    {

        public static event EventHandlerEmpty PlaylistCurrentFileChanged;
        public static int OpenedFileIndex { get; set; } = -1;

        /*private static void FindFileInPlaylist(string file)
        {
            int i = Playlists[0].FindItemlambda(file);
            if (i != -1)
            {
                OpenedFileIndex = i;
                PlaylistCurrentFileChanged?.Invoke();
            };
        }*/

        public static void FindOpenedFileIndex()
        {
            if (Playlists[0].Items == null)
            {
                return;
            }

            int i = Playlists[0].FindItemlambda(NaudioPlayer.Source);
            if (i != -1)
            {
                OpenedFileIndex = i;
            };
            PlaylistCurrentFileChanged?.Invoke();
        }

        #region Playlist
        //
        public static void Initialize()
        {
            Player.PlaybackStateChanged += Player_PlaybackStateChanged;
            Playlists.Add(new PlaylistFile());
            Log.WriteLine("Playlist Intialized");
        }

        private static void Player_PlaybackStateChanged(PlaybackState newPlaybackState)
        {
            if (newPlaybackState == PlaybackState.Ended)
            {
                PlaybackEnded();
            }
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
            Playlists[PlaylistIndex].RemoveItem(index);
        }

        public static void Clear()
        {
            if (NaudioPlayer.Source is not null or "" or " ")
            {
                Playlists[0].ClearSilent();
                Playlists[0].AddItem(NaudioPlayer.Source);
            }
            else
            {
                Playlists[0].Clear();
            }
        }

        private static async void PlaybackEnded()
        {
            NaudioPlayer.Close();
            await NaudioPlayer.OpenAsync();

            Log.WriteLine($"Playlist-Playback Ended / {RepeatMode} Done.");
        }

        #endregion

        public static bool IsFirst(PlaylistFile playlist, string OpenedItem)
        {
            return playlist.FindItemlambda(OpenedItem) == 0;
        }

        public static bool IsLast(PlaylistFile playlist, string OpenedItem)
        {
            return playlist.FindItemlambda(OpenedItem) == playlist.Items.Count - 1;
        }

        public static async void PlayNext()
        {
            if (!IsLast(Playlists[0], NaudioPlayer.Source))
            {
                Player.Source = Playlists[0].Items[OpenedFileIndex + 1].FilePath;
                await Player.OpenAsync();
            }
        }

        public static async void PlayPrevious()
        {
            if (!IsFirst(Playlists[0], NaudioPlayer.Source))
            {
                Player.Source = Playlists[0].Items[OpenedFileIndex - 1].FilePath;
                await Player.OpenAsync();
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
                            Player.Stop();
                            break;
                        case RepeatMode.Close:
                            Player.Close();
                            break;
                        case RepeatMode.CurrentFile:
                            Player.Stop();
                            _ = NaudioPlayer.SeekAsync(0);
                            Player.Play();
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
