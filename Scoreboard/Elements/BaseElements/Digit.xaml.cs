using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Scoreboard.Templates.DigitSets;

namespace Scoreboard.Elements.BaseElements
{
    public partial class Digit : UserControl
    {
        // 7 rows, 4 columns
        private const int Rows = 7;
        private const int Cols = 4;

        private readonly Bulb[,] bulbs = new Bulb[Rows, Cols];

        private int _value;
        private readonly Dictionary<int, int[,]> _activeDigitSet;

        public double Width
        {
            get
            {
                // Total width is the sum of all bulbs and margins
                double bulbSpacing = 2 * Cols * 2; // Margin * 2 (left + right) * number of columns
                double bulbWidth = BulbSize * Cols; // Total bulb width
                return bulbWidth + bulbSpacing; // Add bulbs + margins
            }
        }

        public Digit()
        {
            InitializeComponent();

            var digitSet = new StandardDigitSet();
            _activeDigitSet = digitSet.GetDigitPattern();
              
            CreateGrid();
            RefreshDigit();
        }

        #region Dependency Properties

        // Bulb size
        public static readonly DependencyProperty BulbSizeProperty =
            DependencyProperty.Register(nameof(BulbSize), typeof(double), typeof(Digit),
                new PropertyMetadata(20.0, OnBulbSizeChanged));

        public double BulbSize
        {
            get => (double)GetValue(BulbSizeProperty);
            set => SetValue(BulbSizeProperty, value);
        }

        private static void OnBulbSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var digit = d as Digit;
            digit?.UpdateBulbSizes();
        }

        // Bulb OnColor
        public static readonly DependencyProperty BulbOnColorProperty =
            DependencyProperty.Register(nameof(BulbOnColor), typeof(Brush), typeof(Digit),
                new PropertyMetadata(Brushes.Red, OnBulbOnColorChanged));

        public Brush BulbOnColor
        {
            get => (Brush)GetValue(BulbOnColorProperty);
            set => SetValue(BulbOnColorProperty, value);
        }

        private static void OnBulbOnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var digit = d as Digit;
            digit?.UpdateBulbColors();
        }

        // Bulb OffColor
        public static readonly DependencyProperty BulbOffColorProperty =
            DependencyProperty.Register(nameof(BulbOffColor), typeof(Brush), typeof(Digit),
                new PropertyMetadata(Brushes.DarkRed, OnBulbOffColorChanged));

        public Brush BulbOffColor
        {
            get => (Brush)GetValue(BulbOffColorProperty);
            set => SetValue(BulbOffColorProperty, value);
        }

        private static void OnBulbOffColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var digit = d as Digit;
            digit?.UpdateBulbColors();
        }

        #endregion

        public void SetValue(int value)
        {
            _value = value;
            RefreshDigit();
        }

        private void CreateGrid()
        {
            // Create the row/column definitions
            for (int r = 0; r < Rows; r++)
            {
                RootGrid.RowDefinitions.Add(new RowDefinition
                {
                    Height = GridLength.Auto
                });
            }
            for (int c = 0; c < Cols; c++)
            {
                RootGrid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = GridLength.Auto
                });
            }

            // Create Bulb controls
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    var bulb = new Bulb
                    {
                        Diameter = BulbSize,
                        OnColor = BulbOnColor,
                        OffColor = BulbOffColor,
                        Margin = new Thickness(2),
                    };

                    bulbs[r, c] = bulb;

                    Grid.SetRow(bulb, r);
                    Grid.SetColumn(bulb, c);
                    RootGrid.Children.Add(bulb);
                }
            }
        }

        private void RefreshDigit()
        {
            Debug.WriteLine($"Refresh digit called for {_value}");

            if (!_activeDigitSet.ContainsKey(_value)) return;

            var pattern = _activeDigitSet[_value];
            Debug.WriteLine($"Pattern: {pattern}");
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    Application.Current.Dispatcher.Invoke(() =>  bulbs[r, c].IsOn = (pattern[r, c] == 1));
                }
            }
        }


        private void UpdateBulbSizes()
        {
            // Update the Diameter of all bulbs
            foreach (var bulb in bulbs)
            {
                bulb.Diameter = BulbSize;
            }
        }

        private void UpdateBulbColors()
        {
            // Update the On/Off colors of all bulbs
            foreach (var bulb in bulbs)
            {
                bulb.OnColor = BulbOnColor;
                bulb.OffColor = BulbOffColor;
            }
        }
    }
}
