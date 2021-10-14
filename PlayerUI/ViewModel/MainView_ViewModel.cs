using PlayerUI.ViewModel.Base;
using System.Windows.Input;

namespace PlayerUI.ViewModel
{
    public class MainView_ViewModel : ViewModelBase
    {
        public ICommand Open { get; }
        public ICommand OpenFileLocation { get; }
        public ICommand SwitchToMainView { get; }
        public ICommand Exit { get; }

        public MainView_ViewModel()
        {
            Open = new DelegateCommand(() => Commands.FilePicker.OpenFilePicker());
            OpenFileLocation = new DelegateCommand(() => Helper.OpenFileLocation.Open(App.Player.Source));
            SwitchToMainView = new DelegateCommand(() => Commands.ViewSwitcher.SwitchToMiniView());
            Exit = new DelegateCommand(() => Commands.App.CloseMainWindow());
        }
    }
}
