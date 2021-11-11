using System;
using System.Globalization;
using System.Windows.Data;

namespace PlayerLibrary.Converter
{
    public class VolumeCoverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int result = 0;
            if (value is float Volume)
            {
                try
                {
                    float fval = (float)value;
                    result = (int)(ToDouble(fval) * 100);
                }
                catch (System.Exception)
                {
                    return 0;
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float result = 0;
            if (value is int Volume)
            {
                try
                {
                    int ival = (int)value;
                    double V = (double)ival / 100;
                    V = V < 0 ? 0 : V > 1 ? 1 : V;
                    result = (float)V;
                }
                catch (System.Exception)
                {
                    return 0;
                }
            }

            return result;
        }
        private float ToSingle(double value)
        {
            return (float)value;
        }
        private double ToDouble(float value)
        {
            return value;
        }
    }
}