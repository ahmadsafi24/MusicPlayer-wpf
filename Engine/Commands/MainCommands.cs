﻿using Engine.Enums;
using Engine.Internal;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace Engine.Commands
{
    public static class MainCommands
    {
        public static string Source { get => Player.Source; set { Player.Source = value; } }
        public static double Volume { get => Player.Volume; set => Player.Volume = value; }
        public static string CurrentTimeString { get => Player.CurrentTimeString; }
        public static string TotalTimeString { get => Player.TotalTimeString; }
        public static double CurrentSeconds { get => Player.CurrentTime.TotalSeconds; }
        public static double TotalSeconds { get => Player.TotalTime.TotalSeconds; }
        public static PlaybackState PlaybackState { get => Player.PlaybackState; }

        public static void Initialize()
        {
            Player.Initialize();
        }

        public static async Task OpenAsync() => await Player.OpenAsync();
        public static void Play() => Player.Play();
        public static void Pause() => Player.Pause();
        public static void Close() => Player.Close();
        public static void Stop() => Player.Stop();

        public static async Task SeekAsync(double value) => await Player.SeekAsync(value);

        public static void VolumeUp(double value) => ChangeVolume(Player.Volume += value);
        public static void VolumeDown(double value) => ChangeVolume(Player.Volume -= value);
        public static void ChangeVolume(double newVolume)
        {
            try
            {
                if (newVolume is >= 0 and <= 1)
                {
                    Player.Volume = newVolume;
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message);
            }
        }

        public static void FindCurrentFile()
        {
            PlaylistManager.FindOpenedFileIndex();
        }
    }
}
