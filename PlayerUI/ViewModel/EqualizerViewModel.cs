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
            Player.EqUpdated += () => NotifyPropertyChanged(string.Empty);
        }

        private void loadEq()
        {
            string[] files = Helper.FileOpenPicker.GetFiles("EqPreset");
            if (files != null)
            {
                Player.ImportEq(Equalizer.PresetFromFile(files[0]));
            }
        }

        private void SaveEq()
        {
            Player.ExportEq(@"C:\temp\test.EqPreset");
        }

        private void ResetEq()
        {
            Player.ResetEq();
            NotifyPropertyChanged(string.Empty);
        }

        public bool IsSuperEq
        {
            get => Player.EqualizerMode == EqualizerMode.Super;
            set
            {
                if (value)
                {

                    Player.EqualizerMode = EqualizerMode.Super;
                }
                else
                {
                    Player.EqualizerMode = EqualizerMode.Normal;
                }
                ResetEq();
                Player.ReIntialEq();
                NotifyPropertyChanged(nameof(IsSuperEq));
            }
        }

        public double Band0 { get => Player.GetEqBandGain(0); set { Player.ChangeEq(0, (float)value, false); NotifyPropertyChanged(nameof(Band0)); } }
        public double Band1 { get => Player.GetEqBandGain(1); set { Player.ChangeEq(1, (float)value, false); NotifyPropertyChanged(nameof(Band1)); } }
        public double Band2 { get => Player.GetEqBandGain(2); set { Player.ChangeEq(2, (float)value, false); NotifyPropertyChanged(nameof(Band2)); } }
        public double Band3 { get => Player.GetEqBandGain(3); set { Player.ChangeEq(3, (float)value, false); NotifyPropertyChanged(nameof(Band3)); } }
        public double Band4 { get => Player.GetEqBandGain(4); set { Player.ChangeEq(4, (float)value, false); NotifyPropertyChanged(nameof(Band4)); } }
        public double Band5 { get => Player.GetEqBandGain(5); set { Player.ChangeEq(5, (float)value, false); NotifyPropertyChanged(nameof(Band5)); } }
        public double Band6 { get => Player.GetEqBandGain(6); set { Player.ChangeEq(6, (float)value, false); NotifyPropertyChanged(nameof(Band6)); } }
        public double Band7 { get => Player.GetEqBandGain(7); set { Player.ChangeEq(7, (float)value, false); NotifyPropertyChanged(nameof(Band7)); } }
        public double Band8 { get => Player.GetEqBandGain(8); set { Player.ChangeEq(8, (float)value, false); NotifyPropertyChanged(nameof(Band8)); } }
        public double Band9 { get => Player.GetEqBandGain(9); set { Player.ChangeEq(9, (float)value, false); NotifyPropertyChanged(nameof(Band9)); } }
        public double Band10 { get => Player.GetEqBandGain(10); set { Player.ChangeEq(10, (float)value, false); NotifyPropertyChanged(nameof(Band10)); } }
        public double Band11 { get => Player.GetEqBandGain(11); set { Player.ChangeEq(11, (float)value, false); NotifyPropertyChanged(nameof(Band11)); } }

    }
}