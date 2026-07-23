using System;
using Microsoft.UI.Xaml.Data;

namespace PrimeCheque.Converters
{
    public class AmountFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is decimal amount)
            {
                return $"LKR {amount:N2}";
            }
            return "LKR 0.00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
