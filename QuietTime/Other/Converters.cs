using QuietTime.Models;
using QuietTime.Services;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace QuietTime.Other
{
    /// <summary>
    /// Converts the <see cref="AudioService.IsLocked"/> bool into text for the button on the main screen.
    /// </summary>
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

    /// <summary>
    /// Converts the <see cref="AudioService.IsLocked"/> bool into colours for the main screen background.
    /// </summary>
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Color.FromRgb(242, 162, 171);
            }

            return Color.FromRgb(191, 227, 242);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

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
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member