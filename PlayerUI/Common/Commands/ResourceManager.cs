namespace PlayerUI.Common.Commands
{
    public static class ResourceManager
    {
        private static readonly ResourceDictionary Dark = new() { Source = new Uri(@"pack://application:,,,/PlayerStyles;component/Resource/Theme/Dark.Xaml") };
        private static readonly ResourceDictionary Light = new() { Source = new Uri(@"pack://application:,,,/PlayerStyles;component/Resource/Theme/Light.xaml") };

        public static void LoadThemeResourceDark()
        {
            Application.Current.Resources.MergedDictionaries[0] = Dark;
        }

        public static void LoadThemeResourceLight()
        {
            Application.Current.Resources.MergedDictionaries[0] = Light;
        }
    }
}
