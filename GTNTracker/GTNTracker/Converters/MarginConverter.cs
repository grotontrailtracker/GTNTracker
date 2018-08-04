using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTNTracker.Services;
using Xamarin.Forms;

namespace GTNTracker.Converters
{
    public class MarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Thickness rtnVal = new Thickness(0, 0, 0, 0);
            bool isFullScreen = (bool)value;

            if (!isFullScreen)
            {
                var height = AppStateService.Instance.WindowHeight;
                var width = AppStateService.Instance.WindowWidth;
                var marginHt = 0.15 * height;
                var marginWi = 0.10 * width;
                rtnVal = new Thickness(marginWi, marginHt, marginWi, marginHt);
            }

            return rtnVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
