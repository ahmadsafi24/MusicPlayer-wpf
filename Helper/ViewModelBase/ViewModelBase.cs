using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Helper.ViewModelBase
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            //Task.Run( () =>
            //{
            //await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            //{
            //}));
            //});
        }
    }
}
