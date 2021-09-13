using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MusicApplication.ViewModel.Base;

namespace MusicApplication.Control
{
    /// <summary>
    /// Interaction logic for TitleBar.xaml
    /// </summary>
    public partial class Titlebar : UserControl
    {
        public Titlebar()
        {
            InitializeComponent();
            Loaded += Titlebar_Loaded;
        }

        private void Titlebar_Loaded(object sender, RoutedEventArgs e)
        {
            TitlebarViewModel viewModel = new();
            viewModel.Window = Window.GetWindow(this);
            DataContext = viewModel;
        }

        public string CaptionString
        {
            get => GetValue(CaptionStringProperty) as string;
            set => SetValue(CaptionStringProperty, value);
        }
        public static readonly DependencyProperty CaptionStringProperty = DependencyProperty.Register(
          "CaptionString", typeof(string), typeof(Titlebar), new PropertyMetadata("App"));

    }

    public class TitlebarViewModel : ViewModelBase
    {
        public Window Window { get; set; }
        public ICommand CloseCommand { get; }
        public ICommand MaximizeRestoreCommand { get; }
        public ICommand MinimizeCommand { get; }
        public ICommand DragMoveCommand { get; }
        public ICommand ContextMenuCommand { get; }

        public TitlebarViewModel()
        {
            CloseCommand = new DelegateCommand(() => Helper.Utility.WindowsManager.CloseWindow(Window));
            MaximizeRestoreCommand = new DelegateCommand(() => Helper.Utility.WindowsManager.MaximizeRestore(Window));
            MinimizeCommand = new DelegateCommand(() => Helper.Utility.WindowsManager.Minimize(Window));
            DragMoveCommand = new DelegateCommand(() => Helper.Utility.WindowsManager.DragMove(Window));
            ContextMenuCommand = new DelegateCommand(() => Helper.Utility.WindowsManager.ShowContextMenu(Window));
        }
    }
}
