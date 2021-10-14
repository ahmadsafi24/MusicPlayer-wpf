using System;
using System.Globalization;
using System.Windows.Data;

namespace PlayerLibrary.Converter
{
    public class TotalSecondsToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "";
            if (value is double totalSeconds)
            {
                try
                {
                    TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
                    if (time.TotalMinutes > 60)
                    {
                        result = time.ToString("hh\\:mm\\:ss");
                    }
                    else
                    {
                        result = time.ToString("mm\\:ss");
                    }
                }
                catch (Exception)
                {
                    result = "";
                }
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
