using System;
using Microsoft.UI.Xaml.Data;

namespace PrimeCheque.Converters
{
    public class DateFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateOnly dateOnly)
            {
                return dateOnly.ToString("yyyy-MM-dd");
            }
            if (value is DateTime dateTime)
            {
                return dateTime.ToString("yyyy-MM-dd");
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string s && DateOnly.TryParse(s, out var date))
            {
                return date;
            }
            return DateOnly.FromDateTime(DateTime.Today);
        }
    }
}
