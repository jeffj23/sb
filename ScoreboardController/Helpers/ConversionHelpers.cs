using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace ScoreboardController.Helpers
{
    public static class ConversionHelpers
    {
        /// <summary>
        /// Converts a string to a FontWeight using FontWeightConverter.
        /// Returns FontWeights.Normal if conversion fails.
        /// </summary>
        public static FontWeight ConvertToFontWeight(string fontWeightString)
        {
            var converter = new FontWeightConverter();
            try
            {
                var result = converter.ConvertFromString(null, CultureInfo.InvariantCulture, fontWeightString);
                if (result is FontWeight fontWeight)
                {
                    return fontWeight;
                }
            }
            catch (Exception ex)
            {
                // Optionally log the exception
                System.Diagnostics.Debug.WriteLine($"FontWeight conversion failed: {ex.Message}");
            }

            // Return a default value if conversion fails
            return FontWeights.Normal;
        }

        /// <summary>
        /// Converts a string to a Brush using BrushConverter.
        /// Returns Brushes.Transparent if conversion fails.
        /// </summary>
        public static Brush ConvertToBrush(string brushString)
        {
            var converter = new BrushConverter();
            try
            {
                var result = converter.ConvertFromString(null, CultureInfo.InvariantCulture, brushString);
                if (result is Brush brush)
                {
                    return brush;
                }
            }
            catch (Exception ex)
            {
                // Optionally log the exception
                System.Diagnostics.Debug.WriteLine($"Brush conversion failed: {ex.Message}");
            }

            // Return a default value if conversion fails
            return Brushes.Transparent;
        }

        /// <summary>
        /// Converts a string to a FontWeight using FontWeightConverter asynchronously.
        /// </summary>
        public static FontWeight ConvertToFontWeightSafe(string fontWeightString)
        {
            return ConvertToFontWeight(fontWeightString);
        }
    }
}
