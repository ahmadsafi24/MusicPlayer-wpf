using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            /*
            Button? button = sender as Button;
            if (button == null) return;

            button.Background = new SolidColorBrush(((SolidColorBrush)button.Background).Color);
            Brush brush = button.Background;
            DoubleAnimation animation = new(0.2, new Duration(TimeSpan.FromSeconds(0.2)));
            brush.BeginAnimation(Brush.OpacityProperty, animation);
            */
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {

        }
    }
}
