using Windows.UI.Notifications;

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

            Player.PlaybackSession.EffectContainer.OutSampleProviderChanged+=() =>
            NotifyPropertyChanged(nameof(IsEqualizerEnabled));


        }


        public static void ShowNotification(string title,string message)
        {
            string Image = "";// = "path to image  url is working  filepath not tested";

            string toastXmlString =
            $@"<toast><visual>
            <binding template='ToastGeneric'>
            <text>{title}</text>
            <text>{message}</text>
            <image src='{Image}'/>
            </binding>
            </visual></toast>";

            Windows.Data.Xml.Dom.XmlDocument xmlDoc = new();
            xmlDoc.LoadXml(toastXmlString);

            ToastNotification toastNotification = new(xmlDoc);

            ToastNotifier toastNotifier = ToastNotificationManager.CreateToastNotifier();
            toastNotifier.Show(toastNotification);
        }
        // Enable Disable Equalizer **
        public bool? IsEqualizerEnabled
        {
            get
            {
                return Player?.PlaybackSession.EffectContainer.EnableEqualizer;
            }
        }

        private void TestMethod()
        {
            //ShowNotification("Notification Title","Test Message");

            //Windows.UI.
            //Toggle Equalizer Enable:
            Player.PlaybackSession.EffectContainer.EnableEqualizer = !Player.PlaybackSession.EffectContainer.EnableEqualizer;


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
