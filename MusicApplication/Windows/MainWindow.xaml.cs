using System;
using System.Windows;
namespace MusicApplication.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Initialized += (_, _) => WindowsManager.WindowInitialized(this);
            InitializeComponent();
        }
    }
}
