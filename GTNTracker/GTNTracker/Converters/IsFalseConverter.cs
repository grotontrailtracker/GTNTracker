using System;
using System.Globalization;
using Xamarin.Forms;

namespace GTNTracker.Converters
{
    public class IsFalseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;

            bool testValue = (bool)value;
            return !testValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
