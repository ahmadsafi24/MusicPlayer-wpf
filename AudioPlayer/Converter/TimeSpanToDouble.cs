using System;
using System.Globalization;
using System.Windows.Data;

namespace AudioPlayer.Converter
{
    public class TimeSpanToDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double result = 0;
            if (value is TimeSpan timeSpan)
            {
                try
                {
                    result = timeSpan.TotalSeconds;
                }
                catch (Exception)
                {
                    result = 0;
                }
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan result = TimeSpan.Zero;
            if (value is double totalSeconds)
            {
                try
                {
                    result = TimeSpan.FromSeconds(totalSeconds);
                }
                catch (Exception)
                {
                    result = TimeSpan.Zero;
                }
            }
            return result;
        }
    }
}
