using System;
using System.Windows;
using System.Windows.Input;

namespace MusicApplication.Windows
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            Initialized += (_, _) => WindowsManager.WindowInitialized(this);
            InitializeComponent();
            MouseLeftButtonDown += Window1_MouseLeftButtonDown;
            StateChanged += Window1_StateChanged;
        }

        private void Window1_StateChanged(object sender, EventArgs e)
        {
            WindowState = WindowState.Normal;
            Width = cntnt.ActualWidth;
            Height = cntnt.ActualHeight;
        }

        private void Window1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Helper.WindowsManager.DragMove(this);
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            Helper.ControlboxHelper.RemoveControls(this);
            Helper.IconHelper.RemoveIcon(this);
        }
    }
}
