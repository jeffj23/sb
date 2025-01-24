using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace Scoreboard.Elements.BaseElements;

public class SbPixel
{
    private readonly Point _location;
    private readonly int _radius;
    private readonly SolidColorBrush _onColor;
    private readonly SolidColorBrush _offColor;
    private Path? _path;

    public SbPixel(Point location, int radius, SolidColorBrush onColor, SolidColorBrush offColor)
    {
        _location = location;
        _radius = radius;
        _onColor = onColor;
        _offColor = offColor;
    }

    public void Initialize(Canvas canvas)
    {
        // Create the Path object and initialize it
        _path = new Path
        {
            Fill = _offColor,
            Width = _radius * 2,
            Height = _radius * 2,
            Data = new EllipseGeometry { RadiusX = _radius, RadiusY = _radius },
            Stretch = Stretch.Fill
        };

        // Add the Path to the canvas
        canvas.Dispatcher.Invoke(() =>
        {
            canvas.Children.Add(_path);
            Canvas.SetLeft(_path, _location.X);
            Canvas.SetTop(_path, _location.Y);
        });
    }

    public void On() => _path?.Dispatcher.Invoke(() => _path.Fill = _onColor);

    public void Off() => _path?.Dispatcher.Invoke(() => _path.Fill = _offColor);
}

public class SbPixelManager
{
    private Canvas? _canvas;
    private readonly List<SbPixel> _pixels = new();

    public void RegisterCanvas(Canvas canvas) => _canvas = canvas;

    public bool IsCanvasRegistered() => _canvas != null;

    public SbPixel CreatePixel(Point location, int radius, string onColor, string offColor)
    {
        if (_canvas is null)
        {
            throw new Exception("No canvas has been registered. Use RegisterCanvas method in SbPixelManager");
        }

        var pixel = new SbPixel(location, radius, 
            new SolidColorBrush((Color)ColorConverter.ConvertFromString(onColor)), 
            new SolidColorBrush((Color)ColorConverter.ConvertFromString(offColor)));
        pixel.Initialize(_canvas);
        _pixels.Add(pixel);
        return pixel;
    }

    public void ClearAll()
    {
        _canvas?.Dispatcher.Invoke(() =>
        {
            _canvas.Children.Clear();
            _pixels.Clear();
        });
    }
}