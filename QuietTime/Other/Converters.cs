using System;
using System.Globalization;
using System.Windows.Data;

namespace QuietTime.Other
{
    public class BoolToButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value) return "Unlock";
            return "Lock";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
