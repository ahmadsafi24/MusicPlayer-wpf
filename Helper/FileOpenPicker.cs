using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;

using Microsoft.Win32;

namespace Helper
{
    public sealed class FileOpenPicker
    {
        public static string DialogTitle { get; set; }
        public static string PickerTitle { get; set; }

        public static bool IsFileOk { get; set; }
        public static List<FileDialogCustomPlace> CustomPlacelist { get; set; }
        public FileOpenPicker(string file)
        {

        }

        public FileOpenPicker()
        {
            //init
            //GetFile();
        }

        public static async Task<string[]> GetFileAsync()
        {
            return await Task.Run(() =>
             {
                 OpenFileDialog openFileDialog = new OpenFileDialog()
                 {
                     Filter =
                     "All Supported Audio | *.mp3; *.wma |" +
                     "MP3 | *.mp3 |" +
                     "Wav | *.wav |" +
                     "WMA | *.wma",

                     AddExtension = true,
                    //DefaultExt = ".mp3",
                    Multiselect = true,
                     CheckFileExists = true,
                     CustomPlaces = CustomPlacelist,
                     InitialDirectory = "",
                     Title = PickerTitle

                 };
                 IsFileOk = (bool)openFileDialog.ShowDialog().HasValue;
                //openFileDialog.FileOk = new System.ComponentModel.CancelEventHandler((object sender, System.ComponentModel.CancelEventHandler ea) => { });
                if (IsFileOk)
                 {
                     return openFileDialog.FileNames;

                 }
                 else
                 {
                     return null;
                 }

             });
        }

    }
}
