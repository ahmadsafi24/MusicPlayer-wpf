using PlayerLibrary.Bridge;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PlayerUI.ViewModel
{
    public class EqualizerViewModel : ViewModelBase
    {
        private static Player Player => App.Player;
        private static EqualizerController EqualizerController => App.Player.PlaybackSession.EffectContainer.EqualizerController;
        public DelegateCommand ResetEqCommand { get; }
        public DelegateCommand LoadEqCommand { get; }
        public DelegateCommand SaveEqCommand { get; }
        public EqualizerViewModel()
        {
            ResetEqCommand = new DelegateCommand(ResetEq);
            LoadEqCommand = new DelegateCommand(LoadEq);
            SaveEqCommand = new DelegateCommand(SaveEq);
            EqualizerController.EqUpdated += EqualizerController_EqUpdated;
            NotifyPropertyChanged(null);

            //
        }

        private void EqualizerController_EqUpdated()
        {
            NotifyPropertyChanged(nameof(IsSuperEq));
            NotifyPropertyChanged(string.Empty);
        }

        private void LoadEq()
        {
            string[] files = Helper.FileOpenPicker.GetFiles(fileExtention: "EqPreset");
            if (files != null)
            {
                if (EqualizerController != null)
                {
                    EqualizerController.SetEqPreset(Equalizer.FileToPreset(files[0]));
                }
            }
        }

        private void SaveEq()
        {
            if (EqualizerController != null)
            {
                Equalizer.PresetToFile(EqualizerController.GetEqPreset(), @"C:\temp\test.EqPreset");
            }
        }

        private void ResetEq()
        {
            if (EqualizerController != null)
            {
                PitchFactor = 1;
                NotifyPropertyChanged(nameof(PitchFactor));
                EqualizerController.SetAllBandsGain(0);

                NotifyPropertyChanged(null);
            }
        }

        public bool IsSuperEq
        {
            get => EqualizerController?.EqualizerMode == EqualizerMode.Super;
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
                NotifyPropertyChanged(nameof(IsSuperEq));

            }
        }

        public double? Band0
        {
            get => EqualizerController?.GetBandGain(0);
            set => EqualizerController?.SetBandGain(0, (float)value, true);
        }

        public double? Band1
        {
            get => EqualizerController?.GetBandGain(1);
            set => EqualizerController?.SetBandGain(1, (float)value, true);
        }

        public double? Band2
        {
            get => EqualizerController?.GetBandGain(2);
            set => EqualizerController?.SetBandGain(2, (float)value, true);
        }

        public double? Band3
        {
            get => EqualizerController?.GetBandGain(3);
            set => EqualizerController?.SetBandGain(3, (float)value, true);
        }

        public double? Band4
        {
            get => EqualizerController?.GetBandGain(4);
            set => EqualizerController?.SetBandGain(4, (float)value, true);
        }

        public double? Band5
        {
            get => EqualizerController?.GetBandGain(5);
            set => EqualizerController?.SetBandGain(5, (float)value, true);
        }
        public double? Band6
        {
            get => EqualizerController?.GetBandGain(6);
            set => EqualizerController?.SetBandGain(6, (float)value, true);
        }
        public double? Band7
        {
            get => EqualizerController?.GetBandGain(7);
            set => EqualizerController?.SetBandGain(7, (float)value, true);
        }
        public double? Band8
        {
            get => EqualizerController?.GetBandGain(8);
            set => EqualizerController?.SetBandGain(8, (float)value, true);
        }
        public double? Band9
        {
            get => EqualizerController?.GetBandGain(9);
            set => EqualizerController?.SetBandGain(9, (float)value, true);
        }
        public double? Band10
        {
            get => EqualizerController?.GetBandGain(10);
            set => EqualizerController?.SetBandGain(10, (float)value, true);
        }
        public double? Band11
        {
            get => EqualizerController?.GetBandGain(11);
            set => EqualizerController?.SetBandGain(11, (float)value, true);
        }

        public double PitchFactor
        {
            get => Player.PlaybackSession.EffectContainer.pitchfactor;
            set
            {
                Player.PlaybackSession.EffectContainer.pitchfactor = (float)value;
            }
        }
    }
}