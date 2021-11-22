

namespace PlayerUI.ViewModel
{
    public class MainView_ViewModel : ViewModelBase
    {
        public ICommand Open { get; }

        public ICommand Media_Stop { get; }

        public ICommand OpenFileLocation { get; }
        public ICommand SwitchToMainView { get; }
        public ICommand Exit { get; }

        public MainView_ViewModel()
        {
            Open = new DelegateCommand(() => FilePicker.OpenFilePicker(App.Player));
            Media_Stop = new DelegateCommand(() => App.Player.PlaybackSession.Stop());
            OpenFileLocation = new DelegateCommand(() => Helper.OpenFileLocation.Open(App.Player.PlaybackSession.CurrentTrackFile));
            SwitchToMainView = new DelegateCommand(() => ViewSwitcher.SwitchToMiniView());
            Exit = new DelegateCommand(() => AppCommands.CloseMainWindow());
        }
    }
}
