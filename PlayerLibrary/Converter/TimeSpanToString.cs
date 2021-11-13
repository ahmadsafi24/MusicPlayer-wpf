using System;
using System.Globalization;
using System.Windows.Data;

namespace PlayerLibrary.Converter
{
    public class TimeSpanToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "";
            if (value is TimeSpan time)
            {
                try
                {
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
            return TimeSpan.Zero;
        }

        public static string FromTimespan(TimeSpan timeSpan)
        {
            return timeSpan.ToString("mm\\:ss");
        }
    }
}
