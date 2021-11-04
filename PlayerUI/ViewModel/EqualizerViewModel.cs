using Helper.ViewModelBase;
using PlayerLibrary;
using PlayerLibrary.Preset;
using System.Windows.Input;

namespace PlayerUI.ViewModel
{
    public class EqualizerViewModel : ViewModelBase
    {
        private Player Player => App.Player;
        public ICommand ResetEqCommand { get; }
        public ICommand LoadEqCommand { get; }
        public ICommand SaveEqCommand { get; }

        public EqualizerViewModel()
        {
            ResetEqCommand = new DelegateCommand(ResetEq);
            LoadEqCommand = new DelegateCommand(loadEq);
            SaveEqCommand = new DelegateCommand(SaveEq);
            NotifyPropertyChanged(null);
            Player.EqualizerController.EqUpdated += () => NotifyPropertyChanged(string.Empty);
        }

        private void loadEq()
        {
            string[] files = Helper.FileOpenPicker.GetFiles("EqPreset");
            if (files != null)
            {
                Player.EqualizerController.ImportEq(Equalizer.PresetFromFile(files[0]));
            }
        }

        private void SaveEq()
        {
            Player.EqualizerController.ExportEq(@"C:\temp\test.EqPreset");
        }

        private void ResetEq()
        {
            Player.EqualizerController.ResetEq();
            NotifyPropertyChanged(string.Empty);
        }

        public bool IsSuperEq
        {
            get => Player.EqualizerController.EqualizerMode == EqualizerMode.Super;
            set
            {
                if (value)
                {

                    Player.EqualizerController.EqualizerMode = EqualizerMode.Super;
                }
                else
                {
                    Player.EqualizerController.EqualizerMode = EqualizerMode.Normal;
                }
                ResetEq();
                Player.EqualizerController.ReIntialEq();
                NotifyPropertyChanged(nameof(IsSuperEq));
            }
        }

        public double Band0 { get => Player.EqualizerController.GetEqBandGain(0); set { Player.EqualizerController.ChangeEqBandGain(0, (float)value, false); NotifyPropertyChanged(nameof(Band0)); } }
        public double Band1 { get => Player.EqualizerController.GetEqBandGain(1); set { Player.EqualizerController.ChangeEqBandGain(1, (float)value, false); NotifyPropertyChanged(nameof(Band1)); } }
        public double Band2 { get => Player.EqualizerController.GetEqBandGain(2); set { Player.EqualizerController.ChangeEqBandGain(2, (float)value, false); NotifyPropertyChanged(nameof(Band2)); } }
        public double Band3 { get => Player.EqualizerController.GetEqBandGain(3); set { Player.EqualizerController.ChangeEqBandGain(3, (float)value, false); NotifyPropertyChanged(nameof(Band3)); } }
        public double Band4 { get => Player.EqualizerController.GetEqBandGain(4); set { Player.EqualizerController.ChangeEqBandGain(4, (float)value, false); NotifyPropertyChanged(nameof(Band4)); } }
        public double Band5 { get => Player.EqualizerController.GetEqBandGain(5); set { Player.EqualizerController.ChangeEqBandGain(5, (float)value, false); NotifyPropertyChanged(nameof(Band5)); } }
        public double Band6 { get => Player.EqualizerController.GetEqBandGain(6); set { Player.EqualizerController.ChangeEqBandGain(6, (float)value, false); NotifyPropertyChanged(nameof(Band6)); } }
        public double Band7 { get => Player.EqualizerController.GetEqBandGain(7); set { Player.EqualizerController.ChangeEqBandGain(7, (float)value, false); NotifyPropertyChanged(nameof(Band7)); } }
        public double Band8 { get => Player.EqualizerController.GetEqBandGain(8); set { Player.EqualizerController.ChangeEqBandGain(8, (float)value, false); NotifyPropertyChanged(nameof(Band8)); } }
        public double Band9 { get => Player.EqualizerController.GetEqBandGain(9); set { Player.EqualizerController.ChangeEqBandGain(9, (float)value, false); NotifyPropertyChanged(nameof(Band9)); } }
        public double Band10 { get => Player.EqualizerController.GetEqBandGain(10); set { Player.EqualizerController.ChangeEqBandGain(10, (float)value, false); NotifyPropertyChanged(nameof(Band10)); } }
        public double Band11 { get => Player.EqualizerController.GetEqBandGain(11); set { Player.EqualizerController.ChangeEqBandGain(11, (float)value, false); NotifyPropertyChanged(nameof(Band11)); } }

        private RelayCommand resetBandCommand;
        public ICommand ResetBandCommand => resetBandCommand ??= new RelayCommand(ResetBand);

        private void ResetBand(object parameter)
        {
            int bandnumber = int.Parse(parameter.ToString());

            Player.EqualizerController.ChangeEqBandGain(bandnumber, 0, true);
            NotifyPropertyChanged($"Band{bandnumber}");
        }
    }
}