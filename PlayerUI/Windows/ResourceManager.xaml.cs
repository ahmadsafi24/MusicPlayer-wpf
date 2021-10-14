using System.Windows;
using System.Windows.Media;

namespace PlayerUI.Windows
{
    /// <summary>
    /// Interaction logic for ResourceManager.xaml
    /// </summary>
    public partial class ResourceManager : Window
    {
        public ResourceManager()
        {
            InitializeComponent();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            //Application.Current.Resources.Keys.[KeyTextbox.Text] = Colors.Black;
            //ResourceKey key = (ResourceKey)Resources.FindName(KeyTextbox.Text);
            //object k = Resources[key];

        }
    }
}
