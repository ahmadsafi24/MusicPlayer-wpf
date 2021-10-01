using MusicApplication.ViewModel.Base;
using System.Windows.Input;

namespace MusicApplication.ViewModel
{
    public class ContextMenuMiniViewViewModel : ViewModelBase
    {
        public ICommand Open { get; }
        public ICommand OpenFileLocation { get; }
        public ICommand SwitchToMainView { get; }
        public ICommand Exit { get; }

        public ContextMenuMiniViewViewModel()
        {
            Open = new DelegateCommand(() => Commands.FilePicker.OpenFilePicker());
            OpenFileLocation = new DelegateCommand(() => Helper.OpenFileLocation.Open(App.Player.Source));
            SwitchToMainView = new DelegateCommand(() => Commands.ViewSwitcher.SwitchToMainView());
            Exit = new DelegateCommand(() => Commands.App.CloseMainWindow());
        }
    }
}
