using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

namespace Scoreboard
{
    public class CounterElement : ScoreboardElement
    {
        private int _numDigits;
        private StackPanel _digitsStack;
        private Digit[] _digitControls;

        public CounterElement(
            string elementName,
            int numDigits,
            double bulbSize,
            Brush bulbOnColor,
            Brush bulbOffColor)
            : base(elementName, bulbSize, bulbOnColor, bulbOffColor)
        {
            _numDigits = numDigits;
            InitializeCounterLayout();
        }

        private void InitializeCounterLayout()
        {
            _digitsStack = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center
            };

            _digitControls = new Digit[_numDigits];
            for (int i = 0; i < _numDigits; i++)
            {
                var digit = new Digit
                {
                    BulbSize = BulbSize,
                    BulbOnColor = BulbOnColor,
                    BulbOffColor = BulbOffColor,
                    Margin = new System.Windows.Thickness(2)
                };

                _digitsStack.Children.Add(digit);
                _digitControls[i] = digit;
            }

            ContainerBorder.Child = _digitsStack;
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
