using Engine;
using Engine.Model;
using MusicApplication.ViewModel;
using MusicApplication.ViewModel.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
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
            DataContext = Locator.PlayerVmInstance;
        }
    }


}
