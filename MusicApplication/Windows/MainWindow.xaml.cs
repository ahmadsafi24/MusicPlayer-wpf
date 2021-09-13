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

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            Helper.Utility.IconHelper.RemoveIcon(this);
            if (WindowsManager.Mode == darknet.Mode.Dark)
            {
                Helper.Utility.DwmApi.ToggleImmersiveDarkMode(this, true);
            }
        }


    }
}
