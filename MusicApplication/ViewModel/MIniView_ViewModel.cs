using MusicApplication.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicApplication.ViewModel
{
    public class MiniView_ViewModel:ViewModelBase
    {
        public ICommand Open { get; }
        public ICommand OpenFileLocation { get; }
        public ICommand SwitchToMainView { get; }
        public ICommand Exit { get; }

        public MiniView_ViewModel()
        {
            Open = new DelegateCommand(() => Commands.FilePicker.OpenFilePicker());
            OpenFileLocation = new DelegateCommand(() => Helper.OpenFileLocation.Open(App.Player.Source));
            SwitchToMainView = new DelegateCommand(() => Commands.ViewSwitcher.SwitchToMainView());
            Exit = new DelegateCommand(() => Commands.App.CloseMainWindow());
        }
    }
}
