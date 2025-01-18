using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Scoreboard;

namespace Scoreboard
{
    public interface IReceiveMessages
    {
        void ReceiveMessage(string value);
        public string ElementName { get; set; }
    }

    public abstract class ScoreboardElement : UserControl, IReceiveMessages
    {
        /// <summary>
        /// Each element has a name (like "MainClock", "HomeScore", etc.).
        /// </summary>
        public string ElementName { get; set; }

        /// <summary>
        /// Common settings for the bulbs (size, on color, off color).
        /// </summary>
        public double BulbSize { get; }
        public Brush BulbOnColor { get; }
        public Brush BulbOffColor { get; }

        /// <summary>
        /// The outer container that might show a border, rounded rectangle, etc.
        /// This is where we’ll place the digits, bulbs, etc.
        /// </summary>
        protected Border ContainerBorder { get; private set; }

        public ScoreboardElement(
            string elementName,
            double bulbSize,
            Brush bulbOnColor,
            Brush bulbOffColor)
        {
            ElementName = elementName;
            BulbSize = bulbSize;
            BulbOnColor = bulbOnColor;
            BulbOffColor = bulbOffColor;

            // Create the container
            ContainerBorder = new Border
            {
                Background = Brushes.Transparent, // or some style
                BorderBrush = Brushes.White,      // or any color you want
                BorderThickness = new Thickness(4),
                CornerRadius = new CornerRadius(10), // rounded corners
                Padding = new Thickness(10),
                Margin = new Thickness(20, 10, 20, 10)
            };

            // Put the border in the root of this UserControl
            Content = ContainerBorder;
        }

        /// <summary>
        /// Called when a JSON message sets the value of this element.
        /// e.g. { Element: "MainClock", Value: "20:00" }
        /// The derived class will interpret the string appropriately.
        /// </summary>
        /// <param name="value">String value to be interpreted by the subclass</param>
        public abstract void ReceiveMessage(string value);

    }
}