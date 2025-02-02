using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using Scoreboard.Elements.BaseElements;

namespace Scoreboard.Elements;

public interface IDigit
{
    double Width { get; }
    void SetValue(int value);
    double BulbSize { get; set; }
    Brush BulbOnColor { get; set; }
    Brush BulbOffColor { get; set; }
}


public partial class SegmentDigit : UserControl, IDigit
{


    private readonly SbPixelManager _pixelManager;
    private readonly List<List<SbPixel>> _segments; // Holds SbPixels for each of the 7 segments
    private int _value;

    public double Width => BulbSize * 4; // Approximate width based on segment layout

    public double BulbSize { get; set; }
    public Brush BulbOnColor { get; set; }
    public Brush BulbOffColor { get; set; }

    public SegmentDigit(SbPixelManager pixelManager)
    {
        _pixelManager = pixelManager;
        _segments = new List<List<SbPixel>>(7);

        // Initialize with default properties
        BulbSize = 20;
        BulbOnColor = Brushes.Red;
        BulbOffColor = Brushes.DarkRed;

        InitializeSegments();
    }

    private void InitializeSegments()
    {
        for (int i = 0; i < 7; i++)
        {
            _segments.Add(new List<SbPixel>());
        }

        // Define the layout of each segment using SbPixel
        // Segment 0 (top horizontal)
        CreateSegment(0, new Point(1, 0), 3, true);

        // Segment 1 (top-right vertical)
        CreateSegment(1, new Point(4, 1), 3, false);

        // Segment 2 (bottom-right vertical)
        CreateSegment(2, new Point(4, 5), 3, false);

        // Segment 3 (bottom horizontal)
        CreateSegment(3, new Point(1, 8), 3, true);

        // Segment 4 (bottom-left vertical)
        CreateSegment(4, new Point(0, 5), 3, false);

        // Segment 5 (top-left vertical)
        CreateSegment(5, new Point(0, 1), 3, false);

        // Segment 6 (middle horizontal)
        CreateSegment(6, new Point(1, 4), 3, true);
    }

    private void CreateSegment(int segmentIndex, Point startPoint, int length, bool horizontal)
    {
        for (int i = 0; i < length; i++)
        {
            var location = horizontal
                ? new Point(startPoint.X + i * (int)BulbSize, startPoint.Y)
                : new Point(startPoint.X, startPoint.Y + i * (int)BulbSize);

            var pixel = _pixelManager.CreatePixel(location, (int)(BulbSize / 2), BulbOnColor.ToString(), BulbOffColor.ToString());
            _segments[segmentIndex].Add(pixel);
        }
    }

    public void SetValue(int value)
    {
        _value = value;
        RefreshDigit();
    }

    private void RefreshDigit()
    {
        // Define which segments should light up for each digit (0-9)
        var segmentMap = new Dictionary<int, bool[]>
        {
            { 0, new[] { true, true, true, true, true, true, false } },
            { 1, new[] { false, true, true, false, false, false, false } },
            { 2, new[] { true, true, false, true, true, false, true } },
            { 3, new[] { true, true, true, true, false, false, true } },
            { 4, new[] { false, true, true, false, false, true, true } },
            { 5, new[] { true, false, true, true, false, true, true } },
            { 6, new[] { true, false, true, true, true, true, true } },
            { 7, new[] { true, true, true, false, false, false, false } },
            { 8, new[] { true, true, true, true, true, true, true } },
            { 9, new[] { true, true, true, true, false, true, true } }
        };

        // Turn each segment on/off based on the segment map
        if (segmentMap.TryGetValue(_value, out var segmentsToLight))
        {
            for (int i = 0; i < _segments.Count; i++)
            {
                foreach (var pixel in _segments[i])
                {
                    if (segmentsToLight[i])
                    {
                        pixel.On();
                    }
                    else
                    {
                        pixel.Off();
                    }
                }
            }
        }
    }
}