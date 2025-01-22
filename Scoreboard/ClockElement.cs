using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Scoreboard
{
    public class ClockElement : ScoreboardElement
    {
        private Grid _layoutGrid;
        private bool _useLeadingZeros = false;

        private Digit _digit1; // Tens of minutes
        private Digit _digit2; // Ones of minutes
        private Digit _digit3; // Tens of seconds
        private Digit _digit4; // Ones of seconds

        // Two Bulbs for the colon
        private Bulb _colonTop;
        private Bulb _colonBottom;
        private Bulb _period;

        public ClockElement(
            string elementName,
            double bulbSize,
            Brush bulbOnColor,
            Brush bulbOffColor)
            : base(elementName, bulbSize, bulbOnColor, bulbOffColor)
        {
            InitializeLayout();
        }

        private void InitializeLayout()
        {
            // We'll create a 7x21 grid
            // Rows = 7 (for digit height)
            // Columns = 21 to accommodate digits + spacing + colon
            _layoutGrid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // 7 rows
            for (int r = 0; r < 7; r++)
            {
                _layoutGrid.RowDefinitions.Add(new RowDefinition
                {
                    Height = GridLength.Auto
                });
            }

            // 21 columns
            // columns 0..3   => Digit1
            // column 4       => spacing (BulbSize)
            // columns 5..8   => Digit2
            // column 9       => spacing (BulbSize)
            // column 10      => colon
            // column 11      => spacing (BulbSize)
            // columns 12..15 => Digit3
            // column 16      => spacing (BulbSize)
            // columns 17..20 => Digit4

            for (int c = 0; c < 21; c++)
            {
                var colDef = new ColumnDefinition();

                // Set fixed width = BulbSize for the spacing columns
                if (c == 4 || c == 9 || c == 11 || c == 16)
                {
                    colDef.Width = new GridLength(BulbSize);
                }
                else
                {
                    // auto for the digit columns and the colon column
                    colDef.Width = GridLength.Auto;
                }

                _layoutGrid.ColumnDefinitions.Add(colDef);
            }

            // Create the Digit controls
            // Digit 1 => columns 0..3
            _digit1 = MakeDigit(0, 0, 4);   // row=0, colStart=0, colSpan=4
            // Digit 2 => columns 5..8
            _digit2 = MakeDigit(0, 5, 4);
            // Digit 3 => columns 12..15
            _digit3 = MakeDigit(0, 12, 4);
            // Digit 4 => columns 17..20
            _digit4 = MakeDigit(0, 17, 4);

            // Create the colon bulbs (same diameter as digit bulbs)
            _colonTop = new Bulb
            {
                Diameter = BulbSize,
                OnColor = BulbOnColor,
                OffColor = BulbOffColor,
                Margin = new Thickness(2)
            };
            _colonBottom = new Bulb
            {
                Diameter = BulbSize,
                OnColor = BulbOnColor,
                OffColor = BulbOffColor,
                Margin = new Thickness(2)
            };
            _period = new Bulb
            {
                Diameter = BulbSize,
                OnColor = BulbOnColor,
                OffColor = BulbOffColor,
                Margin = new Thickness(2)
            };

            var p1 = new Bulb
            {
                Diameter = BulbSize,
                OnColor = BulbOnColor,
                OffColor = BulbOffColor,
                Margin = new Thickness(2),
                Visibility = Visibility.Hidden
            };
            var p2 = new Bulb
            {
                Diameter = BulbSize,
                OnColor = BulbOnColor,
                OffColor = BulbOffColor,
                Margin = new Thickness(2),
                Visibility = Visibility.Hidden
            };
            var p3 = new Bulb
            {
                Diameter = BulbSize,
                OnColor = BulbOnColor,
                OffColor = BulbOffColor,
                Margin = new Thickness(2),
                Visibility = Visibility.Hidden
            };
            var p4 = new Bulb
            {
                Diameter = BulbSize,
                OnColor = BulbOnColor,
                OffColor = BulbOffColor,
                Margin = new Thickness(2),
                Visibility = Visibility.Hidden
            };

            // Place them in column 10, rows 1 and 5
            Grid.SetColumn(p1, 10);
            Grid.SetRow(p1, 0);

            Grid.SetColumn(_colonTop, 10);
            Grid.SetRow(_colonTop, 1);

            Grid.SetColumn(p2, 10);
            Grid.SetRow(p2, 2);

            Grid.SetColumn(p3, 10);
            Grid.SetRow(p3, 3);

            Grid.SetColumn(p4, 10);
            Grid.SetRow(p4, 4);

            Grid.SetColumn(_colonBottom, 10);
            Grid.SetRow(_colonBottom, 5);
            
            Grid.SetColumn(_period, 10);
            Grid.SetRow(_period, 6);

            _layoutGrid.Children.Add(p1);
            _layoutGrid.Children.Add(_colonTop);
            _layoutGrid.Children.Add(p2);
            _layoutGrid.Children.Add(p3);
            _layoutGrid.Children.Add(p4);
            _layoutGrid.Children.Add(_colonBottom);
            _layoutGrid.Children.Add(_period);

            // Put the grid into the border
            ContainerBorder.Child = _layoutGrid;
        }

        // Helper: create a Digit control, place it to span 7 rows & colSpan columns
        private Digit MakeDigit(int row, int colStart, int colSpan)
        {
            var digit = new Digit
            {
                BulbSize = BulbSize,
                BulbOnColor = BulbOnColor,
                BulbOffColor = BulbOffColor,
                Margin = new Thickness(2)
            };

            Grid.SetRow(digit, row);
            Grid.SetRowSpan(digit, 7);
            Grid.SetColumn(digit, colStart);
            Grid.SetColumnSpan(digit, colSpan);

            _layoutGrid.Children.Add(digit);

            return digit;
        }

        public void SetValue(string value)
        {
            if (value.Length == 6)
            {
                value = "0" + value; // Add leading "0" for "MM:SS.T
            }

            if (value.Length == 5)
            {
                value = value + ".0";
            }

            var tmin = int.Parse(value.Substring(0, 1));
            var omin = int.Parse(value.Substring(1, 1));
            var tsec = int.Parse(value.Substring(3, 1));
            var osec = int.Parse(value.Substring(4, 1));
            var tent = int.Parse(value.Substring(6, 1));

            if (tmin > 0 || (tmin == 0 && omin > 0))
            {
                if (!_useLeadingZeros)
                {
                    if (tmin == 0)
                    {
                        tmin = -1;
                    }
                }

                _digit1.SetValue(tmin);
                _digit2.SetValue(omin);
                _digit3.SetValue(tsec);
                _digit4.SetValue(osec);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    _colonTop.IsOn = true;
                    _colonBottom.IsOn = true;
                    _period.IsOn = false;
                });

            }
            else
            {
                if (!_useLeadingZeros)
                {
                    if (tsec == 0)
                    {
                        tsec = -1;
                    }
                }

                _digit1.SetValue(tsec);
                _digit2.SetValue(osec);
                _digit3.SetValue(tent);
                _digit4.SetValue(-1);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _colonTop.IsOn = false;
                    _colonBottom.IsOn = false;
                    _period.IsOn = true;
                });
            }
        }

        public override void ReceiveMessage(string value)
        {
            Debug.WriteLine($"Message received in clock element: {value}");
            SetValue(value);
        }

    }
}
