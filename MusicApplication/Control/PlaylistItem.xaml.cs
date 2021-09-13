using Engine;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MusicApplication.Control
{
    /// <summary>
    /// Interaction logic for PlaylistItem.xaml
    /// </summary>
    public partial class PlaylistItem : UserControl
    {
        public PlaylistItem()
        {
            InitializeComponent();
        }

        public string FilePath
        {
            get => GetValue(FilePathProperty) as string;
            set => SetValue(FilePathProperty, value);
        }
        public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register(
          "FilePath", typeof(string), typeof(UserControl), new PropertyMetadata("FilePath"));


        public BitmapImage CoverImage
        {
            get => GetValue(CoverImageProperty) as BitmapImage;
            set => SetValue(CoverImageProperty, value);
        }
        public static readonly DependencyProperty CoverImageProperty = DependencyProperty.Register(
          "CoverImage", typeof(BitmapImage), typeof(UserControl), new PropertyMetadata(null));

        public string TextBlock1
        {
            get => GetValue(TextBlock1Property) as string;
            set => SetValue(TextBlock1Property, value);
        }
        public static readonly DependencyProperty TextBlock1Property = DependencyProperty.Register(
          "TextBlock1", typeof(string), typeof(UserControl), new PropertyMetadata("TextBlock1"));

        public string TextBlock2
        {
            get => GetValue(TextBlock2Property) as string;
            set => SetValue(TextBlock2Property, value);
        }
        public static readonly DependencyProperty TextBlock2Property = DependencyProperty.Register(
          "TextBlock2", typeof(string), typeof(UserControl), new PropertyMetadata("TextBlock2"));

        public string TextBlock3
        {
            get => GetValue(TextBlock3Property) as string;
            set => SetValue(TextBlock3Property, value);
        }
        public static readonly DependencyProperty TextBlock3Property = DependencyProperty.Register(
          "TextBlock3", typeof(string), typeof(UserControl), new PropertyMetadata("TextBlock3"));


    }


}
