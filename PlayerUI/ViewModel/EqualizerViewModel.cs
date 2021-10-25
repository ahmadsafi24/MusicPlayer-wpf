using PlayerLibrary;
using PlayerUI.ViewModel.Base;
using System.Windows.Input;

namespace PlayerUI.ViewModel
{
    public class EqualizerViewModel : ViewModelBase
    {
        private readonly Player Player = App.Player;
        public ICommand ResetEqCommand { get; }

        public EqualizerViewModel()
        {
            ResetEqCommand=new DelegateCommand(ResetEq);
            NotifyPropertyChanged(null);
        }

        private void ResetEq()
        {
            Player.ResetEq();
            NotifyPropertyChanged(string.Empty);
        }

        public int EqMode
        {
            get
            {
                var val = Player.EqualizerMode switch
                {
                    EqualizerMode.Normal => 1,
                    EqualizerMode.Super => 2,
                    EqualizerMode.Disabled => 0,
                    _ => throw new System.NotImplementedException(),
                };
                return val;
            }
            set
            {
                Player.EqualizerMode = value switch
                {
                    0 => EqualizerMode.Disabled,
                    1 => EqualizerMode.Normal,
                    2 => EqualizerMode.Super,
                    _ => throw new System.NotImplementedException(),
                };
                ResetEq();
                Player.ReIntialEq();
            }
        }

        public double Band0 { get => Player.GetEqBandGain(0); set { Player.ChangeEq(0, (float)value); NotifyPropertyChanged(nameof(Band0)); } }
        public double Band1 { get => Player.GetEqBandGain(1); set { Player.ChangeEq(1, (float)value); NotifyPropertyChanged(nameof(Band1)); } }
        public double Band2 { get => Player.GetEqBandGain(2); set { Player.ChangeEq(2, (float)value); NotifyPropertyChanged(nameof(Band2)); } }
        public double Band3 { get => Player.GetEqBandGain(3); set { Player.ChangeEq(3, (float)value); NotifyPropertyChanged(nameof(Band3)); } }
        public double Band4 { get => Player.GetEqBandGain(4); set { Player.ChangeEq(4, (float)value); NotifyPropertyChanged(nameof(Band4)); } }
        public double Band5 { get => Player.GetEqBandGain(5); set { Player.ChangeEq(5, (float)value); NotifyPropertyChanged(nameof(Band5)); } }
        public double Band6 { get => Player.GetEqBandGain(6); set { Player.ChangeEq(6, (float)value); NotifyPropertyChanged(nameof(Band6)); } }
        public double Band7 { get => Player.GetEqBandGain(7); set { Player.ChangeEq(7, (float)value); NotifyPropertyChanged(nameof(Band7)); } }
        public double Band8 { get => Player.GetEqBandGain(8); set { Player.ChangeEq(8, (float)value); NotifyPropertyChanged(nameof(Band8)); } }
        public double Band9 { get => Player.GetEqBandGain(9); set { Player.ChangeEq(9, (float)value); NotifyPropertyChanged(nameof(Band9)); } }
        public double Band10 { get => Player.GetEqBandGain(10); set { Player.ChangeEq(10, (float)value); NotifyPropertyChanged(nameof(Band10)); } }
        public double Band11 { get => Player.GetEqBandGain(11); set { Player.ChangeEq(11, (float)value); NotifyPropertyChanged(nameof(Band11)); } }

    }
}