using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Scoreboard.Models;
using Scoreboard.Templates.DigitSets;
using System.Drawing;
using Scoreboard.Elements.BaseElements;
using Point = System.Drawing.Point;

namespace Scoreboard.Elements
{
    public class VariableMessageElement : ScoreboardElement
    {
        private readonly int _numLetters;
        private readonly SbPixel[,] _pixels;
        private readonly StandardLetterSet _letterSet;
        private readonly Canvas _pixelCanvas;
        private string _textAlignment = "CENTER";
        private int _rows;
        private int _cols;

        public VariableMessageElement(ScoreboardElementModel model, SbPixelManager pixelManager)
            : base(model, pixelManager)
        {
            _numLetters = model.NumDigits; // NumDigits in the model represents the number of letters

            // Initialize the letter set
            _letterSet = new StandardLetterSet();

            // Create a child canvas for the pixels
            _pixelCanvas = new Canvas
            {
                Width = CalculateElementWidth(),
                Height = 11 * Model.BulbSize // 11 rows * bulb size
            };

            PixelManager.RegisterCanvas(_pixelCanvas);

            // Ensure the canvas is registered
            if (!PixelManager.IsCanvasRegistered())
            {
                throw new Exception("No canvas registered with SbPixelManager. Please register a canvas before creating VariableMessageElement.");
            }

            // Add the child canvas to the ContainerBorder
            ContainerBorder.Child = _pixelCanvas;

            // Create the pixel array for the entire grid
            _rows = 11; // 11 rows per letter
            _cols = (_numLetters * 8) - 1; // 7 columns per letter + 1 spacing column
            _pixels = new SbPixel[_rows, _cols];

            // Initialize pixels
            InitializePixels(_rows, _cols, model.BulbSize, model.BulbOnColor, model.BulbOffColor);
        }

        private void InitializePixels(int rows, int cols, double bulbSize, string onColor, string offColor)
        {
            int radius = (int)(bulbSize / 2);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    var location = new Point(c * (int)bulbSize, r * (int)bulbSize); // Calculate pixel location
                    _pixels[r, c] = PixelManager.CreatePixel(location, radius, onColor, offColor);
                    _pixels[r, c].Initialize(_pixelCanvas); // Add pixels to the child canvas
                }
            }
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


            // Calculate the starting column based on alignment
            int messageWidth = message.Length * 8; // Message width in columns
            int startingColumn = CalculateStartingColumn(_cols, messageWidth);

            // Render each letter in the message
            for (int i = 0; i < message.Length; i++)
            {
                var letter = message[i];
                RenderLetter(letter, startingColumn + i * 8); // Each letter starts at an 8-column offset
            }
        }

        private int CalculateStartingColumn(int totalColumns, int messageWidth)
        {
            return _textAlignment switch
            {
                "CENTER" => (totalColumns - messageWidth) / 2, // Center alignment
                "LEFT" => 0, // Left alignment
                _ => 0 // Default to left alignment for unrecognized options
            };
        }

        private void BlankMessage()
        {
            // Turn off all pixels
            foreach (var pixel in _pixels)
            {
                pixel.Off();
            }
        }

        private void RenderLetter(char letter, int columnOffset)
        {
            var pattern = _letterSet.GetLetterPattern(letter);

            for (int r = 0; r < 11; r++) // 11 rows
            {
                for (int c = 0; c < 7; c++) // 7 columns for the letter
                {
                    if (columnOffset + c < _pixels.GetLength(1))
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

        protected override double CalculateElementWidth()
        {
            // Total width = 7 columns per letter + 1 spacing column between letters
            return _numLetters * (7 * Model.BulbSize) + ((_numLetters - 1) * Model.BulbSize);
        }
    }
}
