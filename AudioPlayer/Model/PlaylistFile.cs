using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AudioPlayer.Model
{
    //TODO use LINQ To search delete or.... 
    //use database as storage
    public class PlaylistFile
    {
        public PlaylistFile(List<string> pathlist)
        {
            Items = new();
            foreach (var item in pathlist)
            {
                Items.Add(new() { FilePath = item });
            }
            FireUpdatedEvent();
        }

        private List<AudioFile> _items;
        public List<AudioFile> Items
        {
            get => _items;
            set
            {
                _items = value;
                FireUpdatedEvent();
            }
        }

        internal int FindItemLinq(string searchQuery)
        {
            return Items.Select((AudioFile, index) => (AudioFile, index)).First(x => x.AudioFile.FilePath == searchQuery).index;
        }

        internal int FindItemlambda(string searchQuery)
        {
            return Items.FindIndex(x => x.FilePath == searchQuery);
        }

        internal int FindItemfor(string searchQuery)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                AudioFile item = Items[i];
                if (item.FilePath == searchQuery)
                {
                    return i;
                }
                i += 1;
            }
            return -1;
        }

        internal void AddItem(string File)
        {
            Items.Add(new AudioFile() { FilePath = File });
            RemoveDuplicates();
            FireUpdatedEvent();
        }

        internal void AddItem(AudioFile file)
        {
            Items.Add(file);
            RemoveDuplicates();
            FireUpdatedEvent();
        }

        internal void CreateNewAndAdd(string[] filepathlist)
        {
            Items = CreateNew(filepathlist);
        }

        private static List<AudioFile> CreateNew(string[] pathlist)
        {
            List<AudioFile> list = new();
            Application.Current.Dispatcher.Invoke(delegate
            {
                foreach (string path in pathlist)
                {
                    list.Add(new AudioFile());
                }
            });
            return list;
        }

        internal async Task AddRangeAsync(string[] filepathlist)
        {
            await Task.Run(() =>
            {
                Items.AddRange(filepathlist.Select(path => new AudioFile() { FilePath = path }));
            });
            RemoveDuplicates();
            FireUpdatedEvent();
        }

        internal void Clear()
        {
            Items.Clear();
            FireUpdatedEvent();
        }

        internal void ClearSilent()
        {
            Items.Clear();
            Items = null;
            Items = new();
            GC.Collect();
        }

        internal void RemoveItem(int index)
        {
            Items.RemoveAt(index);
            FireUpdatedEvent();
            Log.WriteLine(index + " removed");
        }

        internal void RemoveItem(AudioFile file)
        {
            _ = Items.Remove(file);
            FireUpdatedEvent();
        }

        internal void RemoveDuplicates()
        {
            IEnumerable<AudioFile> temp = Items.GroupBy(x => x.FilePath).Select(y => y.First());
            _items = new(temp);
        }

        private void FireUpdatedEvent()
        {
            PlaylistUpdated?.Invoke();
            Log.WriteLine("Playlist Updated");
        }

        public event EventHandlerEmpty PlaylistUpdated;
    }
}
