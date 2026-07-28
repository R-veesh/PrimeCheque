using System;
using Microsoft.UI.Xaml.Data;

namespace PrimeCheque.Converters
{
    public class BoolToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isLandscape)
            {
                return isLandscape ? 1 : 0;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is int index)
            {
                return index == 1;
            }
            return false;
        }
    }
}
