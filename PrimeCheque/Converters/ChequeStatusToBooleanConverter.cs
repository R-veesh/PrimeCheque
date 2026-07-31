using System;
using Microsoft.UI.Xaml.Data;
using PrimeCheque.Models;

namespace PrimeCheque.Converters
{
    public class ChequeStatusToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ChequeStatus status && parameter is string targetStatusStr)
            {
                if (Enum.TryParse<ChequeStatus>(targetStatusStr, out var targetStatus))
                {
                    return status == targetStatus;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
