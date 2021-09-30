using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudioPlayer
{
    public class PlaylistV2
    {
        private Player player;

        public PlaylistV2(Player player)
        {
            this.player = player;
        }

        public List<string> pathlist { get; set; } = new();

        public async Task AddRangeAsync(string[] files)
        {
            await Task.Run(() =>
            {
                pathlist.AddRange(files);
            });
            FireUpdatedEvent();
        }

        private void FireUpdatedEvent()
        {
            Updated?.Invoke();
            Log.WriteLine("Playlist Updated");
        }
        public event EventHandlerEmpty Updated;
    }
}
