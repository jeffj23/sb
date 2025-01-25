using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Scoreboard.Elements.BaseElements;
using Scoreboard.Models;

namespace Scoreboard.Elements;

public abstract class ScoreboardElement : UserControl, IReceiveMessages
{
    internal readonly ScoreboardElementModel Model;
    public double Y => Model.PosY ?? 0;
    public string ElementName => Model.ElementName;
    public readonly SbPixelManager PixelManager;

    /// <summary>
    /// The outer container that might show a border, rounded rectangle, etc.
    /// This is where we’ll place the digits, bulbs, etc.
    /// </summary>
    protected Border ContainerBorder { get; private set; }

    public ScoreboardElement(ScoreboardElementModel model, SbPixelManager pixelManager)
    {
        Model = model;
        PixelManager = pixelManager;
        Model.BorderWidth ??= 0;
        Model.BorderHorizontalPadding ??= 10;
        Model.BorderVerticalPadding ??= 10;

        // Create the container
        if (Model.BorderWidth != null)
            ContainerBorder = new Border
            {
                Background = Brushes.Black, // or some style
                BorderBrush = Brushes.White, // or any color you want
                BorderThickness = new Thickness((double) Model.BorderWidth),
                CornerRadius = new CornerRadius(10), // rounded corners
                Padding = new Thickness((double)Model.BorderHorizontalPadding, (double)Model.BorderVerticalPadding, (double)Model.BorderHorizontalPadding, (double)Model.BorderVerticalPadding),
                Margin = new Thickness(20, 10, 20, 10)
            };

        // Put the border in the root of this UserControl
        Content = ContainerBorder;
        Loaded += (o, e) =>
            Debug.WriteLine(
                $"Actual Element Size for {ElementName} is {ContainerBorder.Width} ({ContainerBorder.ActualWidth} actual)");
    }

    /// <summary>
    /// Called when a JSON message sets the value of this element.
    /// e.g. { Element: "MainClock", Value: "20:00" }
    /// The derived class will interpret the string appropriately.
    /// </summary>
    /// <param name="value">String value to be interpreted by the subclass</param>
    public abstract void ReceiveMessage(string value);

    public double CalculateX(double canvasWidth)
    {
        double x = 0;

        double totalElementWidth = ContainerBorder.ActualWidth > 0
            ? ContainerBorder.ActualWidth
            : CalculateElementWidth(Model.ElementType.ToUpperInvariant() == "CLOCK"); // Fallback to calculated width if not yet rendered

        switch (Model.HorizontalAlignment?.ToUpperInvariant() ?? "NONE")
        {
            case "LEFT":
                x = Model.HorizontalOffset ?? 0; // Offset from the left edge
                break;

            case "CENTER":
                x = (canvasWidth - totalElementWidth) / 2;
                break;

            case "RIGHT":
                x = canvasWidth - totalElementWidth - (Model.HorizontalOffset ?? 0);
                break;

            default:
                x = Model.PosX ?? 0; // Fallback to manually set X if alignment isn't used
                break;
        }

        return x;
    }

    // Method to calculate the total width of the element
    private double CalculateElementWidth(bool isClockWithColon)
    {
        if (Model.NumDigits <= 0) return 0;

        // Create a temporary Digit instance to get the width
        var tempDigit = new Digit { BulbSize = Model.BulbSize };

        // Total width = Digit width * number of digits + spacing between digits
        double digitWidth = tempDigit.Width;

        // Add spacing (e.g., 10px between digits)
        double spacing = 32 * (Model.NumDigits - 1);

        // Add ContainerBorder padding (left + right)
        double margin = ContainerBorder.Margin.Left + ContainerBorder.Margin.Right;
        double padding = ContainerBorder.Padding.Left + ContainerBorder.Padding.Right;

        // Add ContainerBorder thickness (left + right)
        double borderThickness = ContainerBorder.BorderThickness.Left + ContainerBorder.BorderThickness.Right;

        // If there is a colon, take that into consideration
        var colonSpacing = (isClockWithColon) ? ((Model.BulbSize + 4) * 3) : 0;

        var totalWidth = (digitWidth * Model.NumDigits) + spacing + 0 + 0 + 0 + colonSpacing;
        // Total width
        return totalWidth;
    }
}