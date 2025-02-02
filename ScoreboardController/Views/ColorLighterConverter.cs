using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ScoreboardController.Views
{
    public class ColorLighterConverter : IValueConverter
    {
        private Color? _originalColor;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            _originalColor = (Color?)value;

            if (value is SolidColorBrush originalBrush)
            {
                Color originalColor = originalBrush.Color;
                // Increase the RGB values to get a lighter color
                byte r = (byte)Math.Min(255, originalColor.R + 30);
                byte g = (byte)Math.Min(255, originalColor.G + 30);
                byte b = (byte)Math.Min(255, originalColor.B + 30);
                return new SolidColorBrush(Color.FromRgb(r, g, b));
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (_originalColor is not null)
            {
                var tempColor = _originalColor ?? Colors.GreenYellow;
                _originalColor = null;
                return new SolidColorBrush(tempColor);
            }

            return new SolidColorBrush(Colors.GreenYellow);
        }
    }
}