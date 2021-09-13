using Engine.Events.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Engine.Model
{
    //TODO use LINQ To search delete or.... 
    //use database as storage
    public class PlaylistFile
    {
        internal PlaylistFile(AudioFile FirstItem)
        {
            FileList = new();
            FileList.Clear();
            FileList.Add(FirstItem);
            FireUpdatedEvent();
        }

        internal PlaylistFile()
        {
            FileList = new();
        }

        private List<AudioFile> _fileList;
        internal List<AudioFile> FileList
        {
            get => _fileList;
            set
            {
                _fileList = value;
                FireUpdatedEvent();
            }
        }

        internal int FindItem(string searchfor)
        {
            int i = 0;
            foreach (AudioFile item in FileList)
            {
                if (item.FilePath == searchfor)
                {
                    return i;
                }
                i += 1;
            }
            return -1;
        }

        internal void AddItem(string File)
        {
            FileList.Add(new AudioFile(File));
            FireUpdatedEvent();
        }

        internal void AddItem(AudioFile file)
        {
            FileList.Add(file);
            FireUpdatedEvent();
        }

        internal void CreateNewAndAdd(string[] filepathlist)
        {
            FileList = CreateNew(filepathlist);
            {//_createNew(filepathlist).ForEach(x => Items.Add(x));
                /*
                 Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    foreach (string path in filepathlist)
                    {
                        Items.Add(new AudioFile(path));
                    }
                });
                */
            }
        }

        private static List<AudioFile> CreateNew(string[] pathlist)
        {
            List<AudioFile> list = new();
            Application.Current.Dispatcher.Invoke(delegate
            {
                foreach (string path in pathlist)
                {
                    list.Add(new AudioFile(path));
                }
            });
            return list;
        }

        internal async void AddRange(string[] filepathlist)
        {
            await Task.Run(() =>
              {
                  foreach (string path in filepathlist)
                  {
                      FileList.Add(new AudioFile(path));
                  }
              });
            FireUpdatedEvent();
        }

        internal void ClearAndNotify()
        {
            FileList.Clear();
            FireUpdatedEvent();
        }

        internal void Clear()
        {
            FileList.Clear();
            FileList = null;
            FileList = new();
        }

        internal void RemoveItem(int index)
        {
            FileList.RemoveAt(index);
            FireUpdatedEvent();
        }

        internal void RemoveItem(AudioFile file)
        {
            _ = FileList.Remove(file);
            FireUpdatedEvent();
        }

        private void FireUpdatedEvent()
        {
            PlaylistUpdated?.Invoke();
        }

        public event EventHandlerNull PlaylistUpdated;
    }
}
