using MusicApplication.ViewModel.Base;
using System.Windows.Input;

namespace MusicApplication.ViewModel
{
    public class MenuViewModel : ViewModelBase
    {
        public ICommand OpenCommand { get; }
        public ICommand TestCommand { get; }

        public MenuViewModel()
        {
            OpenCommand = new DelegateCommand(() => Commands.FilePicker.OpenFilePicker());
            TestCommand = new DelegateCommand(() => Theme.WindowTheme.DarkThemeToggle());
        }
    }
}
