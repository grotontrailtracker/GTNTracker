using System;
using System.Globalization;
using Xamarin.Forms;

namespace GTNTracker.Converters
{
    public class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string rtnVal = string.Empty;

            DateTime? date = value as DateTime?;
            if (date.HasValue)
            {
                var dateValue = date.Value;
                rtnVal = dateValue.ToLocalTime().ToString("T");
            }

            return rtnVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
