using Helper;
using Helper.ViewModelBase;
using PlayerLibrary;
using PlayerLibrary.Preset;
using PlayerLibrary.Core;
using System;
using System.Windows.Input;

namespace PlayerUI.ViewModel
{
    public class EqualizerViewModel : ViewModelBase
    {
        private Player Player => App.Player;
        private EqualizerController EqualizerController => App.Player.EqualizerController;

        public DelegateCommand ResetEqCommand { get; }
        public DelegateCommand LoadEqCommand { get; }
        public DelegateCommand SaveEqCommand { get; }

        public EqualizerViewModel()
        {
            ResetEqCommand = new DelegateCommand(ResetEq);
            LoadEqCommand = new DelegateCommand(loadEq);
            SaveEqCommand = new DelegateCommand(SaveEq);
            NotifyPropertyChanged(null);
            Player.PlaybackSession.NAudioPlayerChanged += PlaybackSession_NAudioPlayerChanged;
            Player.PropertyChanged += EqControllerCreated;
        }

        private void PlaybackSession_NAudioPlayerChanged(Type type)
        {
            Log.WriteLine(type.ToString());
            NotifyPropertyChanged(null);
            if (type == typeof(PlayerLibrary.Core.NAudioPlayer.NAudioPlayerEq))
            {
                if (EqualizerController != null)
                {

                }
            }
        }
        private void EqControllerCreated()
        {
            EqualizerController.EqUpdated += EqControllerUpdated;

        }

        private void EqControllerUpdated()
        {
            NotifyPropertyChanged(null);
        }

        private void loadEq()
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

                EqualizerController.SetAllBandsGain(0);
                NotifyPropertyChanged(null);
            }
        }

        public bool IsSuperEq
        {
            get => EqualizerController?.EqualizerMode == EqualizerMode.Super;
            set
            {
                if (EqualizerController != null)
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
        }

        public double band0 => (double)(EqualizerController?.GetBandGain(0));
        public double Band0 { get => band0; set => EqualizerController?.SetBandGain(0, (float)value, true); }

        public double band1 => (double)(EqualizerController?.GetBandGain(1));
        public double Band1 { get => band1; set => EqualizerController?.SetBandGain(1, (float)value, true); }

        public double band2 => (double)(EqualizerController?.GetBandGain(2));
        public double Band2 { get => band2; set => EqualizerController?.SetBandGain(2, (float)value, true); }

        public double band3 => (double)(EqualizerController?.GetBandGain(3));
        public double Band3 { get => band3; set => EqualizerController?.SetBandGain(3, (float)value, true); }

        public double band4 => (double)(EqualizerController?.GetBandGain(4));
        public double Band4 { get => band4; set => EqualizerController?.SetBandGain(4, (float)value, true); }

        public double Band5 { get => (double)(EqualizerController?.GetBandGain(5)); set { EqualizerController?.SetBandGain(5, (float)value, true); } }
        //uncompleted
        public double Band6 { get => (double)(EqualizerController?.GetBandGain(6)); set { EqualizerController?.SetBandGain(6, (float)value, true); } }
        public double Band7 { get => (double)(EqualizerController?.GetBandGain(7)); set { EqualizerController?.SetBandGain(7, (float)value, true); } }
        public double Band8 { get => (double)(EqualizerController?.GetBandGain(8)); set { EqualizerController?.SetBandGain(8, (float)value, true); } }
        public double Band9 { get => (double)(EqualizerController?.GetBandGain(9)); set { EqualizerController?.SetBandGain(9, (float)value, true); } }
        public double Band10 { get => (double)(EqualizerController?.GetBandGain(10)); set { EqualizerController?.SetBandGain(10, (float)value, true); } }
        public double Band11 { get => (double)(EqualizerController?.GetBandGain(11)); set { EqualizerController?.SetBandGain(11, (float)value, true); } }

        private RelayCommand resetBandCommand;
        public ICommand ResetBandCommand => resetBandCommand ??= new RelayCommand(ResetBand);

        private void ResetBand(object parameter)
        {
            int bandnumber = int.Parse(parameter.ToString());

            EqualizerController?.SetBandGain(bandnumber, 0, true);
            NotifyPropertyChanged($"Band{bandnumber}");
        }
    }
}