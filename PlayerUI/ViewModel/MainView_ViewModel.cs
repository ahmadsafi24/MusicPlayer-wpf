

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
            OpenFileLocation = new DelegateCommand(() =>
            {
                Uri temp = App.Player.PlaybackSession.CurrentTrackFile;
                if (temp.IsFile)
                {
                    Helper.OpenFileLocation.Open(temp.LocalPath);
                }
            });
            SwitchToMainView = new DelegateCommand(() => ViewSwitcher.SwitchToMiniView());
            Exit = new DelegateCommand(() => AppCommands.CloseMainWindow());
        }
    }
}
