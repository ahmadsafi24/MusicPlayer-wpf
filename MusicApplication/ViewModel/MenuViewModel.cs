using MusicApplication.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicApplication.ViewModel
{
    public class MenuViewModel : Base.ViewModelBase
    {
        public ICommand OpenCommand { get; }
        public ICommand TestCommand { get; }

        public MenuViewModel()
        {
            OpenCommand = new DelegateCommand(() => Engine.Commands.MainCommands.OpenFilePicker());
            TestCommand = new DelegateCommand(() => Test());
        }
        private static void Test()
        {
            GC.Collect();
        }
    }
}
