using AudioPlayer.Model;
using MusicApplication.Theme;
using MusicApplication.ViewModel.Base;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;

namespace MusicApplication.ViewModel
{
    public class MenuViewModel : ViewModelBase
    {
        public ICommand OpenCommand { get; }
        public ICommand TestCommand { get; }

        public MenuViewModel()
        {
            OpenCommand = new DelegateCommand(() => SharedStatics.OpenFilePicker());
            TestCommand = new DelegateCommand(() => WindowTheme.DarkThemeToggle());
        }

        private static List<string> getAllFilesPaths(PlaylistFile playlist)
        {
            List<string> paths = new();
            foreach (AudioFile item in playlist.Items)
            {
                paths.Add(item.FilePath);
                Debug.WriteLine(item.FilePath);
            }
            return paths;
        }
    }
}
