using Helper.ViewModelBase;
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
            Open = new DelegateCommand(() => Commands.FilePicker.OpenFilePicker(App.Player));
            OpenFileLocation = new DelegateCommand(() => Helper.OpenFileLocation.Open(App.Player.PlaybackSession.CurrentTrackFile));
            SwitchToMainView = new DelegateCommand(() => Commands.ViewSwitcher.SwitchToMiniView());
            Exit = new DelegateCommand(() => Commands.App.CloseMainWindow());
        }
    }
}
