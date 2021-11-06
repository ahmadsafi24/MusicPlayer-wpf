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
private void AddKeyValue(object key, object value)
        {
            // load the resource dictionary
            var rd = new System.Windows.ResourceDictionary();
            rd.Source = new System.Uri("pack://application:,,,/YOURAssemblyName;component/EnglishResources.xaml", System.UriKind.RelativeOrAbsolute);

            // add the new key with value
            //rd.Add(key, value);
            if (rd.Contains(key))
            {
                rd[key] = value;
            }
            else
            {
                rd.Add(key, value);
            }

            // now you can save the changed resource dictionary
            var settings = new System.Xml.XmlWriterSettings();
            settings.Indent = true;
            var writer = System.Xml.XmlWriter.Create(@"EnglishResources.xaml", settings);
            System.Windows.Markup.XamlWriter.Save(rd, writer);
        }
    }
}
