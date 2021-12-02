using System;
using System.Globalization;
using System.Windows.Data;

namespace PlayerLibrary.Converter
{
    public class VolumeCoverter : IValueConverter
    {
        /// <summary>
        /// float to double (0.1f to 100)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double result = 0;
            if (value is float Volume)
            {
                try
                {
                    result = ToDouble(Volume) * 100;
                }
                catch (System.Exception)
                {
                    return 0;
                }
            }

            return result;
        }

        /// <summary>
        /// double to float (100 to 0.1f)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            float result = 0;
            if (value is double Volume)
            {
                try
                {
                    double V = (Volume) / 100;
                    result = (float)V;
                }
                catch (System.Exception ex)
                {
                    Helper.Log.WriteLine(ex.Message);
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