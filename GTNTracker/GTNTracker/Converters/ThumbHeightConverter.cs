using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GTNTracker.Converters
{
    public class ThumbHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double rtnValue = 66.0; // default value
            double fontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));
            if (fontSize > 0)
            {
                rtnValue = fontSize * 3;
            }

            return rtnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
