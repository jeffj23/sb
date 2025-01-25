using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Scoreboard.Elements.BaseElements;
using Scoreboard.Models;

namespace Scoreboard.Elements
{
    public class CounterElement : ScoreboardElement
    {
        private int _numDigits;
        private Grid _layoutGrid;
        private Digit[] _digitControls;

        public CounterElement(ScoreboardElementModel model, SbPixelManager pixelManager) : base(model, pixelManager)
        {
            _numDigits = model.NumDigits;
            InitializeCounterLayout();
        }

        private void InitializeCounterLayout()
        {
            // Create a Grid for the layout
            _layoutGrid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Add rows for the digits
            for (int r = 0; r < 7; r++) // Assuming 7 rows for digit height
            {
                _layoutGrid.RowDefinitions.Add(new RowDefinition
                {
                    Height = GridLength.Auto
                });
            }

            // Add columns for digits and spacing
            for (int c = 0; c < (_numDigits * 2) - 1; c++)
            {
                _layoutGrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = (c % 2 == 0) ? GridLength.Auto : new GridLength(Model.BulbSize) // Spacing for odd columns
                });
            }

            _digitControls = new Digit[_numDigits];

            // Create and position the digits
            for (int i = 0; i < _numDigits; i++)
            {
                var digit = new Digit
                {
                    BulbSize = Model.BulbSize,
                    BulbOnColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Model.BulbOnColor)),
                    BulbOffColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Model.BulbOffColor)),
                    Margin = new Thickness(2) // Optional
                };

                Grid.SetRow(digit, 0); // Spanning all rows for a digit
                Grid.SetRowSpan(digit, 7);
                Grid.SetColumn(digit, i * 2); // Every second column

                _layoutGrid.Children.Add(digit);
                _digitControls[i] = digit;
            }

            // Set the grid as the child of the border container
            ContainerBorder.Child = _layoutGrid;
        }

        public void SetValue(string value)
        {
            // Split the input by commas if present
            var parts = value.Split(',');

            for (int i = 0; i < _numDigits; i++)
            {
                if (i < parts.Length)
                {
                    var part = parts[i].Trim();

                    if (int.TryParse(part, out int digitValue))
                    {
                        // Assuming that -1 indicates a blank digit
                        _digitControls[i].SetValue(digitValue);
                    }
                    else
                    {
                        // Handle invalid input by setting the digit to blank
                        _digitControls[i].SetValue(-1);
                    }
                }
                else
                {
                    // If not enough parts, set remaining digits to blank
                    _digitControls[i].SetValue(-1);
                }
            }
        }

        public override void ReceiveMessage(string value)
        {
            Debug.WriteLine($"Message received in counter element '{ElementName}': {value}");
            SetValue(value);
        }
    }
}
