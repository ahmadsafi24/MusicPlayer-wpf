using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudioPlayer
{
    public class PlaylistV2
    {
        private readonly Player player;

        public PlaylistV2(Player player)
        {
            this.player = player;
        }

        public List<string> Pathlist { get; set; } = new();

        public async Task AddRangeAsync(string[] files)
        {
            await Task.Run(() =>
            {
                Pathlist.AddRange(files);
            });
            FireUpdatedEvent();
        }

        private void FireUpdatedEvent()
        {
            Updated?.Invoke();
            Log.WriteLine("Playlist Updated");
        }
        public event EventHandlerEmpty Updated;

        public async void PlayPrevious()
        {
            //TODO if its the First File in playlist then Replay it
            await player.OpenAsync();
            player.Play();
        }
    }
}
