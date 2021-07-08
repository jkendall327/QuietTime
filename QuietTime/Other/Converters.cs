using QuietTime.Models;
using QuietTime.Services;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QuietTime.Other
{
    /// <summary>
    /// Converts between a <see cref="DateTime"/> and a <see cref="TimeOnly"/> for integrating a <see cref="Schedule"/> with the date-picker tool on the Schedule page.
    /// </summary>
    public class DateTimeToTimeOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeOnly time = (TimeOnly)value;
            return DateTime.Parse(time.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime time = (DateTime)value;
            return TimeOnly.FromDateTime(time);
        }
    }

    public class BoolToImagePathConverter : IValueConverter
    {
        // todo: this is very ugly, try to find a way to do this in xaml
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolean = (bool)value;

            if (boolean)
            {
                return "../Resources/unlock.png";
            }

            return "../Resources/lock.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}