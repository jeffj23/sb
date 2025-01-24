using Scoreboard.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Scoreboard.Elements;

namespace Scoreboard
{
    public partial class MainWindow : Window
    {
        private ScoreDbContext _context;

        public MainWindow(ScoreDbContext context)
        {
            InitializeComponent();
            _context = context;

            var dispatcher = new MessageDispatcher();

            var server = new WebSocketServer("http://localhost:5000/", dispatcher);
            Task.Run(() => server.StartAsync());

            // Hook up the Loaded event
            Loaded += (s, e) => InitializeScoreboardElements(dispatcher);
        }

        /// <summary>
        /// Initializes the Scoreboard Elements
        /// </summary>
        /// <param name="dispatcher"></param>
        private void InitializeScoreboardElements(MessageDispatcher dispatcher)
        {
            var elements = _context.ScoreboardElements.ToList();

            // Construct UI elements
            foreach (var elemModel in elements)
            {
                // Convert the stored hex/named colors to WPF Brushes
                Brush onColor = ParseColor(elemModel.BulbOnColor);
                Brush offColor = ParseColor(elemModel.BulbOffColor);

                // Handle either a "Clock" or a "Counter" type
                if (elemModel.ElementType.Equals("Clock", StringComparison.OrdinalIgnoreCase))
                {
                    // Create a ClockElement
                    var clock = new ClockElement(elemModel);

                    // Position the clock on the Canvas
                    Canvas.SetLeft(clock, clock.CalculateX(MainGrid.ActualWidth)); // Use ActualWidth here
                    Canvas.SetTop(clock, clock.Y);

                    dispatcher.RegisterElement(clock);
                    MainGrid.Children.Add(clock);
                }
                else if (elemModel.ElementType.Equals("Counter", StringComparison.OrdinalIgnoreCase))
                {
                    var counter = new CounterElement(elemModel);

                    Canvas.SetLeft(counter, counter.CalculateX(MainGrid.ActualWidth)); // Use ActualWidth here
                    Canvas.SetTop(counter, counter.Y);

                    dispatcher.RegisterElement(counter);
                    MainGrid.Children.Add(counter);
                }
                else if (elemModel.ElementType.Equals("VariableMsg", StringComparison.OrdinalIgnoreCase))
                {
                    var messageElement = new VariableMessageElement(elemModel);

                    Canvas.SetLeft(messageElement, messageElement.CalculateX(MainGrid.ActualWidth)); // Use ActualWidth here
                    Canvas.SetTop(messageElement, messageElement.Y);

                    dispatcher.RegisterElement(messageElement);
                    MainGrid.Children.Add(messageElement);
                }
                else
                {
                    // Handle other types (e.g., "Text", "Matrix") if needed
                }
            }

            DrawAlignmentLines();
            dispatcher.DispatchMessage("HomeTeamName", "set,UCOA ACADEMY");
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

        private void DrawAlignmentLines()
        {
            // Clear any existing lines
            var existingLines = MainGrid.Children.OfType<Line>().ToList();
            foreach (var line in existingLines)
            {
                MainGrid.Children.Remove(line);
            }

            // Get canvas dimensions
            double canvasWidth = MainGrid.ActualWidth;
            double canvasHeight = MainGrid.ActualHeight;

            // Ensure canvas dimensions are valid
            if (canvasWidth <= 0 || canvasHeight <= 0)
            {
                return; // Avoid drawing lines if the canvas hasn't rendered yet
            }

            // Calculate percentages
            double[] percentages = { 0.25, 0.50, 0.75 };

            foreach (var percentage in percentages)
            {
                // Create vertical line
                var verticalLine = new Line
                {
                    X1 = canvasWidth * percentage,
                    Y1 = 0,
                    X2 = canvasWidth * percentage,
                    Y2 = canvasHeight,
                    Stroke = Brushes.LightGray, // Light gray for subtle alignment
                    StrokeThickness = 1,
                    StrokeDashArray = new DoubleCollection { 2, 2 } // Dashed line
                };

                // Create horizontal line
                var horizontalLine = new Line
                {
                    X1 = 0,
                    Y1 = canvasHeight * percentage,
                    X2 = canvasWidth,
                    Y2 = canvasHeight * percentage,
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 1,
                    StrokeDashArray = new DoubleCollection { 2, 2 }
                };

                // Add lines to canvas
                MainGrid.Children.Add(verticalLine);
                MainGrid.Children.Add(horizontalLine);

                // Ensure the lines are behind other elements
                Canvas.SetZIndex(verticalLine, -1);
                Canvas.SetZIndex(horizontalLine, -1);
            }
        }

}
}
