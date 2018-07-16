using System;
using System.Globalization;
using Xamarin.Forms;

namespace GTNTracker.Converters
{
    public class IsStartedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color rtnColor = Color.FromHex("#BDBDBD");

            bool isStarted = (bool)value;
            if (isStarted)
            {
                rtnColor = Color.FromHex("#9E9E9E"); // Color.Firebrick; 
            }
 
            return rtnColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
