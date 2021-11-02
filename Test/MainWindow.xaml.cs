using System.Windows;

namespace Test
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContext = new ViewModel();
            InitializeComponent();
        }
    }
}
