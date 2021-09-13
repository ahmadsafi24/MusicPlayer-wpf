﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MusicApplication.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow_Chromeless.xaml
    /// </summary>
    public partial class MainWindowChromeless : Window
    {
        public MainWindowChromeless()
        {
            Initialized += (_, _) => WindowsManager.WindowInitialized(this);
            InitializeComponent();
            SizeChanged += MainWindowChromeless_SizeChanged;
        }

        private async Task SizeChangerAsync(Size NewSize)
        {
            content.Height = NewSize.Height;
            content.Width = NewSize.Width;
            await Task.Delay(350);

        }

        private async void MainWindowChromeless_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            await SizeChangerAsync(e.NewSize);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            Helper.Utility.IconHelper.RemoveIcon(this);
            Helper.Utility.ControlboxHelper.RemoveControls(this);
            if (WindowsManager.Mode == darknet.Mode.Dark)
            {
                Helper.Utility.DwmApi.ToggleImmersiveDarkMode(this, true);
            }
        }

        //private void MainWindow_Chromeless_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //MainView.Height = ActualHeight - 32;
        //MainView.Width = ActualWidth;
        //await ResizeContent(ActualWidth, ActualHeight);
        //await ResizeContent();
        //}

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        //private async Task ResizeContent()
        //{
        //await Task.Delay(1000);
        //MainView.Height = ActualHeight - 32;
        //MainView.Width = ActualWidth;
        //}
        /*
                /// <summary>
                /// Use to Avoid Window Flicker On WindowSizeChanged
                /// </summary>
                /// <param name="width"></param>
                /// <param name="height"></param>
                /// <returns></returns>
                private async Task ResizeContent(double width, double height)
                {
                    await Task.Delay(50);
                    MainView.Height = height - 32;
                    MainView.Width = width;
                }
        */
    }
}
