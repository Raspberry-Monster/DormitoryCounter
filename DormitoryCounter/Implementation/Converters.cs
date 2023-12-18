using System;
using System.Globalization;
using System.Windows.Data;

namespace DormitoryCounter.Implementation
{
    public class DateTimeToDateOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                return DateOnly.FromDateTime(dateTime);
            }
            if (value is DateOnly dateOnly)
            {
                return dateOnly.ToDateTime(TimeOnly.MinValue);
            }
            throw new ArgumentException("Is Not DateTime Or DateOnly.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                return DateOnly.FromDateTime(dateTime);
            }
            if (value is DateOnly dateOnly)
            {
                return dateOnly.ToDateTime(TimeOnly.MinValue);
            }
            throw new ArgumentException("Is Not DateTime Or DateOnly.");
        }
    }
}
