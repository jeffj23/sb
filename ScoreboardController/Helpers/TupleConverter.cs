using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ScoreboardController.Helpers
{
    public class TupleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2)
            {
                return new Tuple<object, RoutedEventArgs>(values[0], (RoutedEventArgs)values[1]);
            }

            return null!;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}