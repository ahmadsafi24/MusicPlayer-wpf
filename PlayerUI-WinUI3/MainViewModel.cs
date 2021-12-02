using Helper.ViewModelBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;

namespace PlayerUI_WinUI3
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion


        private string testPropertyBind;
        public string TestPropertyBind
        {
            get => testPropertyBind;
            set
            {
                testPropertyBind = value;
                NotifyPropertyChanged(nameof(TestPropertyBind));
            }
        }
        public MainViewModel()
        {
        }

        private RelayCommand testCommand;
        public ICommand TestCommand => testCommand ??= new RelayCommand(Test);

        private void Test()
        {
            TestPropertyBind = "123";
            System.Windows.MessageBox.Show("ok");
        }

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }

            return false;
        }

        public string TitlebarTitle { get => "AppTitle"; }
    }

}
