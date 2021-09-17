using MusicApplication.ViewModel.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Engine;
using Engine.Model;
using Engine.Commands;
using Engine.Events;
using System.Windows.Media.Imaging;

namespace MusicApplication.Control
{
    /// <summary>
    /// Interaction logic for MediaInfoCard.xaml
    /// </summary>
    public partial class MediaInfoCard : UserControl
    {
        public MediaInfoCard()
        {
            InitializeComponent();
        }
    }

    public class MediaInfoCardViewModel : ViewModelBase
    {
        public ICommand OpenCurrentFileLocationCommand { get; }
        public ICommand SelectCurrentFileInPlaylistCommand { get; }

        public MediaInfoCardViewModel()
        {
            OpenCurrentFileLocationCommand = new DelegateCommand(() => Shared.OpenCurrentFileLocation());
            SelectCurrentFileInPlaylistCommand = new DelegateCommand(() => MainCommands.FindCurrentFile());
            AllEvents.CurrentTimeChanged += AudioPlayer_CurrentTimeChanged;
            AllEvents.PlaybackStateChanged += async (Engine.Enums.PlaybackState playbackState) =>
            {
                if (playbackState == Engine.Enums.PlaybackState.Opened)
                {

                    await Task.Run(() =>
                    {
                        TagFile = (AudioFile)(PlaylistManager.PlaylistItems?[PlaylistManager.OpenedFileIndex]);
                        NotifyPropertyChanged(nameof(TagFile));

                        NotifyPropertyChanged(nameof(TotalTimeString));
                        NotifyPropertyChanged(nameof(CurrentTimeString));

                        Engine.Utility.Class2 class2 = new();
                        class2.OnImageCreated += (BitmapImage ti) => { img = ti; NotifyPropertyChanged(nameof(Cover)); };
                        class2.CreateImage(MainCommands.Source);
                    });
                }
            };
        }

        private async Task AudioPlayer_CurrentTimeChanged(TimeSpan Time)
        {
            await Task.Run(() =>
            {
                NotifyPropertyChanged(nameof(CurrentTimeString));
            });
        }

        BitmapImage img = new();
        public BitmapImage Cover
        {
            get
            {


                return img;
                //return Engine.Utility.Class1.ExtractCover(MainCommands.Source);
            }
        }

#pragma warning disable CA1822 // Mark members as static
        public AudioFile TagFile { get; set; }

        public string CurrentTimeString => MainCommands.CurrentTimeString;
        public string TotalTimeString => MainCommands.TotalTimeString;
#pragma warning restore CA1822 // Mark members as static
    }

}
