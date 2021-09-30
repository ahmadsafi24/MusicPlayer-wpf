using AudioPlayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudioPlayer
{
    public class PlaylistManager
    {
        private readonly Player PublicPlayer;
        public PlaylistManager(Player player)
        {
            PublicPlayer = player;
            PublicPlayer.PlaybackStateChanged += Player_PlaybackStateChanged;
            Log.WriteLine("Playlist Intialized");
        }

        public event EventHandlerEmpty PlaylistCurrentFileChanged;
        public int OpenedFileIndex { get; set; } = -1;

        /*private  void FindFileInPlaylist(string file)
        {
            int i = Playlists[0].FindItemlambda(file);
            if (i != -1)
            {
                OpenedFileIndex = i;
                PlaylistCurrentFileChanged?.Invoke();
            };
        }*/

        public void FindOpenedFileIndex()
        {
            if (Playlists[0].Items == null)
            {
                return;
            }

            int i = Playlists[0].FindItemlambda(PublicPlayer.Source);
            if (i != -1)
            {
                OpenedFileIndex = i;
            };
            PlaylistCurrentFileChanged?.Invoke();
        }

        #region Playlist


        private void Player_PlaybackStateChanged(PlaybackState newPlaybackState)
        {
            if (newPlaybackState == PlaybackState.Ended)
            {
                PlaybackEnded();
            }
        }

        public List<PlaylistFile> Playlists { get; set; } = new();

        public List<AudioFile> PlaylistItems { get => Playlists[0].Items; }

        public void Add(int PlaylistIndex, string File)
        {
            Playlists[PlaylistIndex].AddItem(File);
        }

        public async Task AddRangeAsync(int PlaylistIndex, string[] Files)
        {
            await Playlists[PlaylistIndex].AddRangeAsync(Files);
        }

        public void Remove(int PlaylistIndex, int index)
        {
            Playlists[PlaylistIndex].RemoveItem(index);
        }

        public void Clear()
        {
            if (PublicPlayer.Source is not null or "" or " ")
            {
                Playlists[0].ClearSilent();
                Playlists[0].AddItem(PublicPlayer.Source);
            }
            else
            {
                Playlists[0].Clear();
            }
        }

        private async void PlaybackEnded()
        {
            PublicPlayer.Close();
            await PublicPlayer.OpenAsync();

            Log.WriteLine($"Playlist-Playback Ended / {RepeatMode} Done.");
        }

        #endregion

        public bool IsFirst(PlaylistFile playlist, string OpenedItem)
        {
            return playlist.FindItemlambda(OpenedItem) == 0;
        }

        public bool IsLast(PlaylistFile playlist, string OpenedItem)
        {
            return playlist.FindItemlambda(OpenedItem) == playlist.Items.Count - 1;
        }

        public async void PlayNext()
        {
            if (!IsLast(Playlists[0], PublicPlayer.Source))
            {
                PublicPlayer.Source = Playlists[0].Items[OpenedFileIndex + 1].FilePath;
                await PublicPlayer.OpenAsync();
            }
        }

        public async void PlayPrevious()
        {
            if (!IsFirst(Playlists[0], PublicPlayer.Source))
            {
                PublicPlayer.Source = Playlists[0].Items[OpenedFileIndex - 1].FilePath;
                await PublicPlayer.OpenAsync();
            }

        }

        internal RepeatMode RepeatMode { get; set; } = RepeatMode.CurrentFile;
        internal async Task RepeaterAsync()
        {
            await Task.Run(() =>
            {
                if (true)
                {
                    switch (RepeatMode)
                    {
                        case RepeatMode.Stop:
                            PublicPlayer.Stop();
                            break;
                        case RepeatMode.Close:
                            PublicPlayer.Close();
                            break;
                        case RepeatMode.CurrentFile:
                            PublicPlayer.Stop();
                            _ = PublicPlayer.SeekAsync(0);
                            PublicPlayer.Play();
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
