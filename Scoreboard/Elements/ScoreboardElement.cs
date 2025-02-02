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

    public double CalculateX(double canvasWidth)
    {
        double x = 0;

        // Use the abstract method to calculate width
        double totalElementWidth = CalculateElementWidth();

        switch (Model.HorizontalAlignment?.ToUpperInvariant() ?? "NONE")
        {
            case "LEFT":
                x = Model.HorizontalOffset ?? 0; // Offset from the left edge
                break;

            case "CENTER":
                x = (canvasWidth - totalElementWidth) / 2 + (Model.HorizontalOffset ?? 0); // Center with offset
                break;

            case "RIGHT":
                x = canvasWidth - totalElementWidth - (Model.HorizontalOffset ?? 0); // Offset from the right edge
                break;

            default:
                x = Model.PosX ?? 0; // Fallback to manually set X if alignment isn't used
                break;
        }

        return x;
    }

    protected abstract double CalculateElementWidth();

    public abstract void ReceiveMessage(string value);
}