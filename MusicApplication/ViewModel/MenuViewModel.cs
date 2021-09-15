using Engine;
using Engine.Model;
using MusicApplication.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicApplication.ViewModel
{
    public class MenuViewModel : Base.ViewModelBase
    {
        public ICommand OpenCommand { get; }
        public ICommand TestCommand { get; }

        public MenuViewModel()
        {
            OpenCommand = new DelegateCommand(() => Engine.Commands.MainCommands.OpenFilePicker());
            TestCommand = new DelegateCommand(() => SavetoDb());
        }

        private static void ReadFromDb()
        {

            //await PlaylistManager.AddRangeAsync(0, items);
        }

        private static void SavetoDb()
        {
        }

        private static List<string> getAllFilesPaths(PlaylistFile playlist)
        {
            List<string> paths = new();
            foreach (var item in playlist.Items)
            {
                paths.Add(item.FilePath);
                Debug.WriteLine(item.FilePath);
            }
            return paths;
        }
    }
}
