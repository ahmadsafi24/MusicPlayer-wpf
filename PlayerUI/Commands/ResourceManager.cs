using System;
using System.Windows;

namespace PlayerUI.Commands
{
    public static class ResourceManager
    {
        private static readonly ResourceDictionary Dark = new() { Source = new Uri("..\\Resource\\Theme\\Dark.Xaml", UriKind.Relative) };
        private static readonly ResourceDictionary Light = new() { Source = new Uri("..\\Resource\\Theme\\Light.Xaml", UriKind.Relative) };

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
