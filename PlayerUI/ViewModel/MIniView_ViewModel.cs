using PlayerUI.ViewModel.Base;
using System.Windows.Input;

namespace PlayerUI.ViewModel
{
    public class MiniView_ViewModel : ViewModelBase
    {
        public ICommand Open { get; }
        public ICommand OpenFileLocation { get; }
        public ICommand SwitchToMainView { get; }
        public ICommand Exit { get; }

        public MiniView_ViewModel()
        {
            Open = new DelegateCommand(() => Commands.FilePicker.OpenFilePicker(App.Player));
            OpenFileLocation = new DelegateCommand(() => Helper.OpenFileLocation.Open(App.Player.Source));
            SwitchToMainView = new DelegateCommand(() => Commands.ViewSwitcher.SwitchToMainView());
            Exit = new DelegateCommand(() => Commands.App.CloseMainWindow());
        }
    }
}
