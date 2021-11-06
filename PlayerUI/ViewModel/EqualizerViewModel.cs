using Helper.ViewModelBase;
using PlayerLibrary;
using PlayerLibrary.Preset;
using PlayerLibrary.Shell;
using System.Windows.Input;

namespace PlayerUI.ViewModel
{
    public class EqualizerViewModel : ViewModelBase
    {
        private SoundPlayer Player => App.Player;
        private EqualizerController EqualizerController => App.Player.EqualizerController;

        public ICommand ResetEqCommand { get; }
        public ICommand LoadEqCommand { get; }
        public ICommand SaveEqCommand { get; }

        public EqualizerViewModel()
        {
            ResetEqCommand = new DelegateCommand(ResetEq);
            LoadEqCommand = new DelegateCommand(loadEq);
            SaveEqCommand = new DelegateCommand(SaveEq);
            NotifyPropertyChanged(null);
            EqualizerController.EqUpdated += () => NotifyPropertyChanged(string.Empty);
        }

        private void loadEq()
        {
            string[] files = Helper.FileOpenPicker.GetFiles(fileExtention: "EqPreset");
            if (files != null)
            {
                EqualizerController.SetEqPreset(Equalizer.FileToPreset(files[0]));
            }
        }

        private void SaveEq()
        {
            Equalizer.PresetToFile(EqualizerController.GetEqPreset(), @"C:\temp\test.EqPreset");
        }

        private void ResetEq()
        {
            EqualizerController.SetAllBandsGain(0);
            NotifyPropertyChanged(string.Empty);
        }

        public bool IsSuperEq
        {
            get => EqualizerController.EqualizerMode == EqualizerMode.Super;
            set
            {
                if (value)
                {

                    EqualizerController.EqualizerMode = EqualizerMode.Super;
                }
                else
                {
                    EqualizerController.EqualizerMode = EqualizerMode.Normal;
                }
                //ResetEq();
                EqualizerController.RequestResetEqController();
                NotifyPropertyChanged(nameof(IsSuperEq));
            }
        }

        public double Band0 { get => EqualizerController.GetBandGain(0); set { EqualizerController.SetBandGain(0, (float)value, false); NotifyPropertyChanged(nameof(Band0)); } }
        public double Band1 { get => EqualizerController.GetBandGain(1); set { EqualizerController.SetBandGain(1, (float)value, false); NotifyPropertyChanged(nameof(Band1)); } }
        public double Band2 { get => EqualizerController.GetBandGain(2); set { EqualizerController.SetBandGain(2, (float)value, false); NotifyPropertyChanged(nameof(Band2)); } }
        public double Band3 { get => EqualizerController.GetBandGain(3); set { EqualizerController.SetBandGain(3, (float)value, false); NotifyPropertyChanged(nameof(Band3)); } }
        public double Band4 { get => EqualizerController.GetBandGain(4); set { EqualizerController.SetBandGain(4, (float)value, false); NotifyPropertyChanged(nameof(Band4)); } }
        public double Band5 { get => EqualizerController.GetBandGain(5); set { EqualizerController.SetBandGain(5, (float)value, false); NotifyPropertyChanged(nameof(Band5)); } }
        public double Band6 { get => EqualizerController.GetBandGain(6); set { EqualizerController.SetBandGain(6, (float)value, false); NotifyPropertyChanged(nameof(Band6)); } }
        public double Band7 { get => EqualizerController.GetBandGain(7); set { EqualizerController.SetBandGain(7, (float)value, false); NotifyPropertyChanged(nameof(Band7)); } }
        public double Band8 { get => EqualizerController.GetBandGain(8); set { EqualizerController.SetBandGain(8, (float)value, false); NotifyPropertyChanged(nameof(Band8)); } }
        public double Band9 { get => EqualizerController.GetBandGain(9); set { EqualizerController.SetBandGain(9, (float)value, false); NotifyPropertyChanged(nameof(Band9)); } }
        public double Band10 { get => EqualizerController.GetBandGain(10); set { EqualizerController.SetBandGain(10, (float)value, false); NotifyPropertyChanged(nameof(Band10)); } }
        public double Band11 { get => EqualizerController.GetBandGain(11); set { EqualizerController.SetBandGain(11, (float)value, false); NotifyPropertyChanged(nameof(Band11)); } }

        private RelayCommand resetBandCommand;
        public ICommand ResetBandCommand => resetBandCommand ??= new RelayCommand(ResetBand);

        private void ResetBand(object parameter)
        {
            int bandnumber = int.Parse(parameter.ToString());

            EqualizerController.SetBandGain(bandnumber, 0, true);
            NotifyPropertyChanged($"Band{bandnumber}");
        }
    }
}