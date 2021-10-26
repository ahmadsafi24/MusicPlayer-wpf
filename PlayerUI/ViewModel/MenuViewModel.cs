﻿using PlayerUI.ViewModel.Base;
using System.Windows.Input;

namespace PlayerUI.ViewModel
{
    public class MenuViewModel : ViewModelBase
    {
        public ICommand OpenCommand { get; }
        public ICommand ToggleDarkModeCommand { get; }
        public ICommand SwitchToMiniViewCommand { get; }
        public ICommand SwitchToNormalWindowCommand { get; }
        public ICommand SwitchToBlurWindowCommand { get; }
        public MenuViewModel()
        {
            OpenCommand = new DelegateCommand(() => Commands.FilePicker.OpenFilePicker());
            ToggleDarkModeCommand = new DelegateCommand(() => Commands.WindowTheme.DarkThemeToggle());
            SwitchToMiniViewCommand = new DelegateCommand(() => Commands.ViewSwitcher.SwitchToMiniView());
            SwitchToBlurWindowCommand=new DelegateCommand(()=>Commands.ViewSwitcher.SwitchToBlurWindow());
        SwitchToNormalWindowCommand=new DelegateCommand(()=>Commands.ViewSwitcher.SwitchToNormalWindow());
        }
    }
}
