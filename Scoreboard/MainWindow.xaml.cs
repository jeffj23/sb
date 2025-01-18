using Scoreboard.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Scoreboard
{
    public partial class MainWindow : Window
    {
        private ScoreDbContext _context;

        public MainWindow(ScoreDbContext context)
        {
            InitializeComponent();
            _context = context;

            var elements = _context.ScoreboardElements.ToList();
            var dispatcher = new MessageDispatcher();

            var server = new WebSocketServer("http://localhost:5000/", dispatcher);
            Task.Run(() => server.StartAsync());

            // Construct UI elements
            foreach (var elemModel in elements)
            {
                // Convert the stored hex/named colors to WPF Brushes
                Brush onColor = ParseColor(elemModel.BulbOnColor);
                Brush offColor = ParseColor(elemModel.BulbOffColor);

                // We'll handle either a "Clock" or a "Counter" type
                if (elemModel.ElementType.Equals("Clock", StringComparison.OrdinalIgnoreCase))
                {
                    // Create a ClockElement
                    // If you want 3 or 4 digits, you can pass that or read from elemModel.NumDigits
                    var clock = new ClockElement(
                        elementName: elemModel.ElementName,
                        bulbSize: elemModel.BulbSize,
                        bulbOnColor: onColor,
                        bulbOffColor: offColor
                    );



                    // Position the clock. If using a Grid, you might rely on row/column definitions.
                    // If using absolute positioning on a Canvas:
                     Canvas.SetLeft(clock, elemModel.PosX);
                     Canvas.SetTop(clock, elemModel.PosY);
                    // myCanvas.Children.Add(clock);

                    // For now, we’ll just add it to MainGrid
                    dispatcher.RegisterElement(clock);
                    MainGrid.Children.Add(clock);
                }
                else if (elemModel.ElementType.Equals("Counter", StringComparison.OrdinalIgnoreCase))
                {
                    var counter = new CounterElement(
                        elementName: elemModel.ElementName,
                        numDigits: elemModel.NumDigits,
                        bulbSize: elemModel.BulbSize,
                        bulbOnColor: onColor,
                        bulbOffColor: offColor
                    );

                    Canvas.SetLeft(counter, elemModel.PosX);
                    Canvas.SetTop(counter, elemModel.PosY);

                    dispatcher.RegisterElement(counter);
                    MainGrid.Children.Add(counter);
                }
                else
                {
                    // Potential future element types, e.g. "Text", "Matrix"
                    // For now, do nothing or log a warning
                }
            }
        }

        /// <summary>
        /// Parses a color string from the database (e.g. "#FFFF00", "#3C3C00", "Yellow", etc.)
        /// into a WPF Brush. Returns Brushes.White if parsing fails.
        /// </summary>
        private Brush ParseColor(string colorString)
        {
            if (string.IsNullOrWhiteSpace(colorString))
                return Brushes.White;

            try
            {
                var converter = new BrushConverter();
                var brush = (Brush)converter.ConvertFromString(colorString);
                return brush ?? Brushes.White;
            }
            catch
            {
                // If conversion fails, default to white
                return Brushes.White;
            }
        }
    }
}
