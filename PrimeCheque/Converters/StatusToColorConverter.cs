using System;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using PrimeCheque.Models;

namespace PrimeCheque.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ChequeStatus status)
            {
                var color = status switch
                {
                    ChequeStatus.Draft => Colors.Orange,
                    ChequeStatus.Approved => Colors.DeepSkyBlue,
                    ChequeStatus.Printed => Colors.Green,
                    ChequeStatus.Void => Colors.Red,
                    ChequeStatus.StopPayment => Colors.DarkRed,
                    _ => Colors.Gray
                };
                return new SolidColorBrush(color);
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
