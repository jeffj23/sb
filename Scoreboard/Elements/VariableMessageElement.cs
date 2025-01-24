using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Scoreboard.Models;
using Scoreboard.Templates.DigitSets;

namespace Scoreboard.Elements
{
    public class VariableMessageElement : ScoreboardElement
    {
        private readonly int _numLetters;
        private readonly Grid _layoutGrid;
        private readonly StandardLetterSet _letterSet;
        private readonly Bulb[,] _bulbs;

        public VariableMessageElement(ScoreboardElementModel model)
            : base(model)
        {
            _numLetters = model.NumDigits; // NumDigits in the model represents the number of letters
            _letterSet = new StandardLetterSet();

            // Initialize the layout grid
            _layoutGrid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Create rows and columns for the grid
            for (int r = 0; r < 11; r++) // 11 rows for each letter
            {
                _layoutGrid.RowDefinitions.Add(new RowDefinition
                {
                    Height = GridLength.Auto
                });
            }

            for (int c = 0; c < (_numLetters * 8) - 1; c++) // 7 columns per letter + 1 spacing column
            {
                _layoutGrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = GridLength.Auto
                });
            }

            // Create the bulb array for the entire grid
            _bulbs = new Bulb[11, _numLetters * 8];

            for (int r = 0; r < 11; r++)
            {
                for (int c = 0; c < (_numLetters * 8) - 1; c++)
                {
                    var bulb = new Bulb
                    {
                        Diameter = model.BulbSize,
                        OnColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(model.BulbOnColor)),
                        OffColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(model.BulbOffColor)),
                        Margin = new Thickness(1) // Optional spacing around each bulb
                    };

                    _bulbs[r, c] = bulb;

                    Grid.SetRow(bulb, r);
                    Grid.SetColumn(bulb, c);
                    _layoutGrid.Children.Add(bulb);
                }
            }

            // Add the grid to the container
            ContainerBorder.Child = _layoutGrid;
        }

        public override void ReceiveMessage(string value)
        {
            Debug.WriteLine($"Message received in VariableMessageElement '{ElementName}': {value}");

            var parts = value.Split(',');

            if (parts.Length == 2)
            {
                var command = parts[0].Trim().ToLowerInvariant();
                var message = parts[1].Trim().ToUpperInvariant();

                switch (command)
                {
                    case "set":
                        SetMessage(message);
                        break;

                    case "blank":
                        BlankMessage();
                        break;

                    default:
                        Debug.WriteLine($"Unknown command: {command}");
                        break;
                }
            }
        }

        private void SetMessage(string message)
        {
            // Truncate or pad the message to fit the number of letters
            if (message.Length > _numLetters)
            {
                message = message.Substring(0, _numLetters);
            }
            else if (message.Length < _numLetters)
            {
                message = message.PadRight(_numLetters, ' ');
            }

            // Render each letter in the message
            for (int i = 0; i < _numLetters; i++)
            {
                var letter = message[i];
                RenderLetter(letter, i * 8); // Each letter starts at an 8-column offset
            }
        }

        private void BlankMessage()
        {
            // Turn off all bulbs
            foreach (var bulb in _bulbs)
            {
                bulb.IsOn = false;
            }
        }

        private void RenderLetter(char letter, int columnOffset)
        {
            var pattern = _letterSet.GetLetterPattern(letter);

            for (int r = 0; r < 11; r++) // 11 rows
            {
                for (int c = 0; c < 7; c++) // 7 columns for the letter
                {
                    if (columnOffset + c < _bulbs.GetLength(1))
                    {
                        _bulbs[r, columnOffset + c].IsOn = (pattern[r, c] == 1);
                    }
                }
            }
        }

        public double CalculateElementWidth()
        {
            // Total width = 7 columns per letter + 1 spacing column between letters
            return _numLetters * (7 * Model.BulbSize) + ((_numLetters - 1) * Model.BulbSize);
        }
    }
}
