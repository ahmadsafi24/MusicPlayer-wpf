using Helper.ViewModelBase;
using System;
using System.Windows.Input;
using System.Windows.Media;

namespace PlayerUI.ViewModel
{
    public class MenuViewModel : ViewModelBase
    {
        public ICommand TestCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand ToggleDarkModeCommand { get; }
        public ICommand SwitchToMiniViewCommand { get; }
        public ICommand SwitchToNormalWindowCommand { get; }
        public ICommand SwitchToBlurWindowCommand { get; }
        public MenuViewModel()
        {
            TestCommand = new DelegateCommand(() => TestMethod());
            OpenCommand = new DelegateCommand(() => Commands.FilePicker.OpenFilePicker(App.Player));
            ToggleDarkModeCommand = new DelegateCommand(() => Commands.WindowTheme.DarkThemeToggle());
            SwitchToMiniViewCommand = new DelegateCommand(() => Commands.ViewSwitcher.SwitchToMiniView());
            SwitchToBlurWindowCommand = new DelegateCommand(() => Commands.ViewSwitcher.SwitchToBlurWindow());
            SwitchToNormalWindowCommand = new DelegateCommand(() => Commands.ViewSwitcher.SwitchToNormalWindow());
        }
        private void TestMethod()
        {
            Random rnd = new Random();
            Color randomColor = Color.FromRgb((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256));

            ChangeResource("Color.Background.Static", randomColor);

        }
        private void ChangeResource(string key, Color color)
        {
            object obj = App.Current.TryFindResource(key);

            if (obj != null)
            {
                App.Current.Resources[key] = color;
            }
        }

    }
}
