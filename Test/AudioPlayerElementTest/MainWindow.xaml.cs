using System.Windows;

namespace AudioPlayerElementTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AllowDrop = true;
            DragEnter += (_, e) => e.Effects = DragDropEffects.All;
            Drop += (_, e) =>
            {
                string[] dropitems = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                APE.play(dropitems[0]);
            };
        }
    }
}
