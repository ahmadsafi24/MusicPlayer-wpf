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
            Open = new DelegateCommand(() => FilePicker.OpenFilePicker(App.Player));
            OpenFileLocation = new DelegateCommand(() => Helper.OpenFileLocation.Open(App.Player.PlaybackSession.CurrentTrackFile.AbsolutePath));
            SwitchToMainView = new DelegateCommand(() => ViewSwitcher.SwitchToMainView());
            Exit = new DelegateCommand(() => AppCommands.CloseMainWindow());
        }
    }
}
