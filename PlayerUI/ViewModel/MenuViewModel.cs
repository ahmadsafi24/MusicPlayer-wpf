namespace PlayerUI.ViewModel
{
    public class MenuViewModel : ViewModelBase
    {
        private static Player Player => App.Player;


        public ICommand TestCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand ToggleDarkModeCommand { get; }
        public ICommand SwitchToMiniViewCommand { get; }
        public ICommand SwitchToNormalWindowCommand { get; }
        public ICommand SwitchToBlurWindowCommand { get; }
        public MenuViewModel()
        {
            TestCommand = new DelegateCommand(() => TestMethod());
            OpenCommand = new DelegateCommand(() => FilePicker.OpenFilePicker(App.Player));
            ToggleDarkModeCommand = new DelegateCommand(() => WindowTheme.DarkThemeToggle());
            SwitchToMiniViewCommand = new DelegateCommand(() => ViewSwitcher.SwitchToMiniView());
            SwitchToBlurWindowCommand = new DelegateCommand(() => ViewSwitcher.SwitchToBlurWindow());
            SwitchToNormalWindowCommand = new DelegateCommand(() => ViewSwitcher.SwitchToNormalWindow());
        }

        // Enable Disable Equalizer
        private static void TestMethod()
        {
            /*if (Player.PlaybackSession.NAudioPlayerType == typeof(PlayerLibrary.Core.NAudioPlayer.NAudioPlayerEq))
            {
                Player.DisableEqualizerController();
            }
            else
            {
                Player.EnableEqualizerController();
            }*/


            /*Random rnd = new Random();
            Color randomColor = Color.FromRgb((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256));

            ChangeResource("Color.Background.Static", randomColor);*/

        }
        private static void ChangeResource(string key, Color color)
        {
            object obj = App.Current.TryFindResource(key);

            if (obj != null)
            {
                App.Current.Resources[key] = color;
            }
        }

        private static void AddKeyValue(object key, object value)
        {
            // load the resource dictionary
            ResourceDictionary rd = new()
            {
                Source = new Uri("pack://application:,,,/YOURAssemblyName;component/EnglishResources.xaml", UriKind.RelativeOrAbsolute)
            };
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
            XmlWriterSettings settings = new()
            {
                Indent = true
            };
            XmlWriter writer = XmlWriter.Create(@"EnglishResources.xaml", settings);
            XamlWriter.Save(rd, writer);
        }
    }
}
