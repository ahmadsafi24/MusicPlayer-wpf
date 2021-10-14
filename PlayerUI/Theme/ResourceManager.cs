using System;
using System.Windows;

namespace PlayerUI.Theme
{
    public static class ResourceManager
    {
        private static readonly ResourceDictionary Dark = new() { Source = new Uri("..\\Resource\\Theme\\Dark.Xaml", UriKind.Relative) };
        private static readonly ResourceDictionary Light = new() { Source = new Uri("..\\Resource\\Theme\\Light.Xaml", UriKind.Relative) };
        private static readonly ResourceDictionary Brushs = new() { Source = new Uri("..\\Resource\\Brushs.Xaml", UriKind.Relative) };

        private static readonly ResourceDictionary Geometry = new() { Source = new Uri("..\\Resource\\Style\\Geometry.Xaml", UriKind.Relative) };
        private static readonly ResourceDictionary OpenSans = new() { Source = new Uri("..\\Resource\\Font\\OpenSans.Xaml", UriKind.Relative) };
        private static readonly ResourceDictionary TextBlock = new() { Source = new Uri("..\\Resource\\Style\\TextBlock.Xaml", UriKind.Relative) };
        private static readonly ResourceDictionary Scrollbar = new() { Source = new Uri("..\\Resource\\Style\\Scrollbar.Xaml", UriKind.Relative) };
        private static readonly ResourceDictionary ScrollViewer = new() { Source = new Uri("..\\Resource\\Style\\ScrollViewer.Xaml", UriKind.Relative) };
        private static readonly ResourceDictionary Listview = new() { Source = new Uri("..\\Resource\\Style\\Listview.Xaml", UriKind.Relative) };
        private static readonly ResourceDictionary Slider = new() { Source = new Uri("..\\Resource\\Style\\Slider.Xaml", UriKind.Relative) };
        private static readonly ResourceDictionary Menu = new() { Source = new Uri("..\\Resource\\Style\\Menu.Xaml", UriKind.Relative) };
        private static readonly ResourceDictionary Button = new() { Source = new Uri("..\\Resource\\Style\\Button.Xaml", UriKind.Relative) };
        private static readonly ResourceDictionary Tooltip = new() { Source = new Uri("..\\Resource\\Style\\Tooltip.Xaml", UriKind.Relative) };
        private static readonly ResourceDictionary Other = new() { Source = new Uri("..\\Resource\\Style\\Other.Xaml", UriKind.Relative) };


        public static void LoadResources()
        {
            Application.Current.Resources.MergedDictionaries.Clear();
            /*if (WindowTheme.IsDark)
            { Application.Current.Resources.MergedDictionaries.Add(Dark); }
            else
            { Application.Current.Resources.MergedDictionaries.Add(Light); }

            _dictReload();*/

        }

        public static void LoadThemeResourceDark()
        {
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(Dark);
            _dictReload();
        }

        public static void LoadThemeResourceLight()
        {
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(Light);
            _dictReload();
        }
        private static void _dictReload()
        {
            Application.Current.Resources.MergedDictionaries.Add(Brushs);
            Application.Current.Resources.MergedDictionaries.Add(Geometry);
            Application.Current.Resources.MergedDictionaries.Add(OpenSans);
            Application.Current.Resources.MergedDictionaries.Add(TextBlock);
            Application.Current.Resources.MergedDictionaries.Add(Scrollbar);
            Application.Current.Resources.MergedDictionaries.Add(ScrollViewer);
            Application.Current.Resources.MergedDictionaries.Add(Listview);
            Application.Current.Resources.MergedDictionaries.Add(Slider);
            Application.Current.Resources.MergedDictionaries.Add(Menu);
            Application.Current.Resources.MergedDictionaries.Add(Button);
            Application.Current.Resources.MergedDictionaries.Add(Tooltip);
            Application.Current.Resources.MergedDictionaries.Add(Other);
        }
    }
}
