using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Scoreboard.Elements.BaseElements;
using Scoreboard.Models;
using System.Drawing;
using Point = System.Drawing.Point;
using Scoreboard.Templates.DigitSets;

namespace Scoreboard.Elements
{
    public class ClockElement : ScoreboardElement
    {
        private readonly Canvas _pixelCanvas;
        private readonly SbPixel[,] _pixels; // 2D array for all pixels
        private readonly int _rows = 7;
        private readonly int _cols = 21; // To accommodate digits + spacing + colon
        private bool _useLeadingZeros = false;
        private readonly Dictionary<int, int[,]> _digitSet;

        public ClockElement(ScoreboardElementModel model, SbPixelManager pixelManager) : base(model, pixelManager)
        {
            // Create a child canvas for the pixels
            _pixelCanvas = new Canvas
            {
                Width = (_cols * Model.BulbSize) + ((_cols - 1) * 4),
                Height = (_rows * Model.BulbSize) + ((_rows - 1) * 4)
            };

            PixelManager.RegisterCanvas(_pixelCanvas);

            // Add the canvas to the ContainerBorder
            ContainerBorder.Child = _pixelCanvas;

            // Create the pixel array for the entire grid
            _pixels = new SbPixel[_rows, _cols];
            var set = new StandardDigitSet();
            _digitSet = set.GetDigitPattern();

            // Initialize the grid with pixels
            InitializePixels();
        }

        private void InitializePixels()
        {
            int radius = (int)(Model.BulbSize / 2);

            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _cols; c++)
                {
                    if (c == 4 || c == 9 || c == 11 || c == 16 ) continue;
                    if (c == 10 && (r == 0 || r == 2 || r == 3 || r == 4)) continue;
                    var location = new Point(c * (int)Model.BulbSize + (c * 4), r * (int)Model.BulbSize + (r * 4));
                    _pixels[r, c] = PixelManager.CreatePixel(location, radius, Model.BulbOnColor, Model.BulbOffColor);
                    _pixels[r, c].Initialize(_pixelCanvas); // Add pixels to the child canvas
                }
            }
        }

        public void SetValue(string value)
        {
            if (value.Length == 6)
            {
                value = "0" + value; // Add leading "0" for "MM:SS.T"
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

                RenderDigit(tmin, 0); // Digit 1
                RenderDigit(omin, 5); // Digit 2
                RenderDigit(tsec, 12); // Digit 3
                RenderDigit(osec, 17); // Digit 4

                RenderColon(true); // Turn colon on
                RenderPeriod(false); // Turn period off
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

                RenderDigit(tsec, 0); // Digit 1
                RenderDigit(osec, 5); // Digit 2
                RenderDigit(tent, 12); // Digit 3
                BlankDigit(17); // No digit in position 4

                RenderColon(false); // Turn colon off
                RenderPeriod(true); // Turn period on
            }
        }

        private void RenderDigit(int value, int columnOffset)
        {
            var pattern = _digitSet[value];


            for (int r = 0; r < 7; r++) // 7 rows
            {
                for (int c = 0; c < 4; c++) // 4 columns per digit
                {
                    if (columnOffset + c < _cols)
                    {
                        if (pattern[r, c] == 1)
                        {
                            _pixels[r, columnOffset + c].On();
                        }
                        else
                        {
                            _pixels[r, columnOffset + c].Off();
                        }
                    }
                }
            }
        }

        private void BlankDigit(int columnOffset)
        {
            for (int r = 0; r < 7; r++) // 7 rows
            {
                for (int c = 0; c < 4; c++) // 4 columns per digit
                {
                    if (columnOffset + c < _cols)
                    {
                        _pixels[r, columnOffset + c].Off();
                    }
                }
            }
        }

        private void RenderColon(bool isOn)
        {
            int colonColumn = 10;

            if (isOn)
            {
                _pixels[1, colonColumn].On(); // Top colon bulb
                _pixels[5, colonColumn].On(); // Bottom colon bulb
            }
            else
            {
                _pixels[1, colonColumn].Off();
                _pixels[5, colonColumn].Off();
            }
        }

        private void RenderPeriod(bool isOn)
        {
            int periodColumn = 10;

            if (isOn)
            {
                _pixels[6, periodColumn].On(); // Period bulb
            }
            else
            {
                _pixels[6, periodColumn].Off();
            }
        }

        public override void ReceiveMessage(string value)
        {
            Debug.WriteLine($"Message received in clock element: {value}");
            SetValue(value);
        }

        protected override double CalculateElementWidth()
        {
            // Clock-specific width calculation
            double digitWidth = Model.BulbSize * 4; // Width of one digit
            double spacing = 32 * (Model.NumDigits - 1); // Spacing between digits
            double colonSpacing = (Model.BulbSize + 4) * 3; // Colon spacing
            double padding = ContainerBorder.Padding.Left + ContainerBorder.Padding.Right;
            double borderThickness = ContainerBorder.BorderThickness.Left + ContainerBorder.BorderThickness.Right;

            return (digitWidth * Model.NumDigits) + spacing + colonSpacing + padding + borderThickness;
        }
    }
}
