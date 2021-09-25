﻿using Engine;
using MusicApplication.ViewModel.Base;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MusicApplication.Control
{
    /// <summary>
    /// Interaction logic for EqualizerControl.xaml
    /// </summary>
    public partial class EqualizerControl : UserControl
    {
        public EqualizerControl()
        {
            InitializeComponent();
        }
    }

    public class EqualizerViewModel : ViewModelBase
    {
        public EqualizerViewModel()
        {
            NotifyPropertyChanged(null);
        }

        private DelegateCommand resetEqCommand;
        public ICommand ResetEqCommand => resetEqCommand ??= new DelegateCommand(ResetEq);

        private void ResetEq()
        {
            Player.ResetEq();
            NotifyPropertyChanged(null);
        }

        public double Band0 { get => Player.GetBandGain(0); set => Player.ChangeEq(0, (float)value); }
        public double Band1 { get => Player.GetBandGain(1); set => Player.ChangeEq(1, (float)value); }
        public double Band2 { get => Player.GetBandGain(2); set => Player.ChangeEq(2, (float)value); }
        public double Band3 { get => Player.GetBandGain(3); set => Player.ChangeEq(3, (float)value); }
        public double Band4 { get => Player.GetBandGain(4); set => Player.ChangeEq(4, (float)value); }
        public double Band5 { get => Player.GetBandGain(5); set => Player.ChangeEq(5, (float)value); }
        public double Band6 { get => Player.GetBandGain(6); set => Player.ChangeEq(6, (float)value); }
        public double Band7 { get => Player.GetBandGain(7); set => Player.ChangeEq(7, (float)value); }
    }
}