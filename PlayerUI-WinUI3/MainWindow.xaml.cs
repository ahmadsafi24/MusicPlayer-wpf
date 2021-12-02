using Helper.ViewModelBase;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Popups;
using WinRT;
using static PlayerUI_WinUI3.Config;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PlayerUI_WinUI3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly PlayerLibrary.Player player = new();

        private bool isslidermouseover;
        public MainWindow()
        {
            InitializeComponent();
            player.PlaybackSession.TimelineController.TimePositionChanged += TimelineController_TimePositionChanged;
            player.PlaybackSession.OnMessageLogged += PlaybackSession_OnMessageLogged;
            player.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
        }

        private void PlaybackSession_PlaybackStateChanged(PlayerLibrary.PlaybackState playbackState)
        {
            DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
            {
                switch (playbackState)
                {
                    case PlayerLibrary.PlaybackState.Failed:
                        smtc.PlaybackStatus = MediaPlaybackStatus.Closed;
                        break;
                    case PlayerLibrary.PlaybackState.Opened:
                        smtc.PlaybackStatus = MediaPlaybackStatus.Paused;
                        slidertime.Maximum = player.PlaybackSession.TimelineController.Total.TotalSeconds;
                        break;
                    case PlayerLibrary.PlaybackState.Paused:
                        smtc.PlaybackStatus = MediaPlaybackStatus.Paused;
                        break;
                    case PlayerLibrary.PlaybackState.Playing:
                        smtc.PlaybackStatus = MediaPlaybackStatus.Playing;
                        break;
                    case PlayerLibrary.PlaybackState.Stopped:
                        smtc.PlaybackStatus = MediaPlaybackStatus.Stopped;
                        break;
                    case PlayerLibrary.PlaybackState.Ended:
                        smtc.PlaybackStatus = MediaPlaybackStatus.Stopped;
                        break;
                    case PlayerLibrary.PlaybackState.Closed:
                        smtc.PlaybackStatus = MediaPlaybackStatus.Closed;
                        break;
                    default:
                        break;
                }

            });
        }

        private void PlaybackSession_OnMessageLogged(string message)
        {
            DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
            {
                _ = ShowMessageAsync(message);

            });
        }

        readonly MediaPlayer mediaPlayer = new MediaPlayer();
        SystemMediaTransportControls smtc;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
            {
                var root = (sender as Button).XamlRoot.Content;
                (root as Grid).RequestedTheme = ElementTheme.Dark;
                //gridroot.RequestedTheme = ElementTheme.Dark;
                //buttonopen.RequestedTheme = ElementTheme.Dark;
            });


            smtc = mediaPlayer.SystemMediaTransportControls;
            mediaPlayer.CommandManager.IsEnabled = false;

            //var smtc = Windows.Media.SystemMediaTransportControlsInterop.GetForWindow(Hwnd);
            smtc.IsEnabled = true;
            smtc.IsNextEnabled = true;
            smtc.IsPreviousEnabled = true;
            smtc.IsPlayEnabled = true;
            smtc.IsPauseEnabled = true;
            smtc.IsChannelUpEnabled = true;
            smtc.PlaybackStatus = MediaPlaybackStatus.Stopped;
            smtc.DisplayUpdater.Update();

            smtc.ButtonPressed += Smtc_ButtonPressed;
            smtc.IsEnabled = true;


            player.PlaybackSession.Open(new Uri(@"G:\1399 downloads\New folder\Downloads\Telegram Desktop\Hichkas - Chi Shenidi (Ft. Fadaei) [Niet Remix].mp3"));

            //MediaPlaybackCommandManager.IsEnabled
            /*StorageFile file =await StorageFile.GetFileFromPathAsync(@"G:\1399 downloads\music\Epicure - Haji Daskhosh [320].mp3");
            Player.Source = MediaSource.CreateFromStorageFile(file);
            Player.Play();*/

            //player.PlaybackSession.Open(new Uri(@"G:\1399 downloads\music\Epicure - Haji Daskhosh [320].mp3"));
            //player.PlaybackSession.Play();








        }

        private void Smtc_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    player.PlaybackSession.Play();
                    smtc.IsEnabled = true;
                    smtc.IsNextEnabled = true;
                    smtc.IsPreviousEnabled = true;
                    smtc.IsPlayEnabled = true;
                    smtc.IsPauseEnabled = true;
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    player.PlaybackSession.Pause();
                    break;
                case SystemMediaTransportControlsButton.Stop:
                    player.PlaybackSession.Stop();
                    break;
                case SystemMediaTransportControlsButton.Next:
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    break;
                default:
                    _ = ShowMessageAsync("this button is not supported");
                    return;
            }
            smtc.DisplayUpdater.Update();
        }

        private void TimelineController_TimePositionChanged(TimeSpan timeSpan)
        {
            DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
            {
                if (isslidermouseover == false)
                {
                    slidertime.Value = timeSpan.TotalSeconds;
                    Title = timeSpan.ToString();

                }
            });
        }

        private void slidertime_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (isslidermouseover)
            {
                player.PlaybackSession.TimelineController.Seek(TimeSpan.FromSeconds(e.NewValue));
            }
        }








        // Static Methods
        public static async Task ShowMessageAsync(string message)
        {
            MessageDialog dialog = new(message);
            WinRT.Interop.InitializeWithWindow.Initialize(dialog, WindowHelper.Hwnd);
            await dialog.ShowAsync();

        }

        private void volumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var vc = new PlayerLibrary.Converter.VolumeCoverter();
            player.PlaybackSession.VolumeController.Volume = (float)vc.ConvertBack(e.NewValue, null, null, null);
        }

        private void slidertime_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Pointer ptr = e.Pointer;
            if (ptr.IsInContact)
            {
                isslidermouseover = true;
                player.PlaybackSession.TimelineController.Seek(TimeSpan.FromSeconds(slidertime.Value));
            }
            else
            {
                isslidermouseover = false;
            }
        }
    }

    public static class Config
    {
        /// <summary>
        /// Window Handle Required For Dialogs
        /// </summary>



    }
}
