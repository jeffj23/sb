using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Scoreboard
{
    public partial class Bulb : UserControl
    {
        public Bulb()
        {
            InitializeComponent();
            UpdateColor();
        }

        // Diameter
        public static readonly DependencyProperty DiameterProperty =
            DependencyProperty.Register(nameof(Diameter), typeof(double), typeof(Bulb),
                new PropertyMetadata(20.0, OnDiameterChanged));

        public double Diameter
        {
            get => (double)GetValue(DiameterProperty);
            set => SetValue(DiameterProperty, value);
        }

        private static void OnDiameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // You can handle additional logic here if needed
        }

        // IsOn
        public static readonly DependencyProperty IsOnProperty =
            DependencyProperty.Register(nameof(IsOn), typeof(bool), typeof(Bulb),
                new PropertyMetadata(false, OnIsOnChanged));

        public bool IsOn
        {
            get => (bool)GetValue(IsOnProperty);
            set => SetValue(IsOnProperty, value);
        }

        private static void OnIsOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine($"OnIsOnChanged called");
            var bulb = d as Bulb;
            bulb?.UpdateColor();
        }

        // OnColor
        public static readonly DependencyProperty OnColorProperty =
            DependencyProperty.Register(nameof(OnColor), typeof(Brush), typeof(Bulb),
                new PropertyMetadata(Brushes.Red, OnOnColorChanged));

        public Brush OnColor
        {
            get => (Brush)GetValue(OnColorProperty);
            set => SetValue(OnColorProperty, value);
        }

        private static void OnOnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bulb = d as Bulb;
            bulb?.UpdateColor();
        }

        // OffColor
        public static readonly DependencyProperty OffColorProperty =
            DependencyProperty.Register(nameof(OffColor), typeof(Brush), typeof(Bulb),
                new PropertyMetadata(Brushes.DarkRed, OnOffColorChanged));

        public Brush OffColor
        {
            get => (Brush)GetValue(OffColorProperty);
            set => SetValue(OffColorProperty, value);
        }

        private static void OnOffColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine($"OnOffColorChanged called");
            var bulb = d as Bulb;
            bulb?.UpdateColor();
        }

        private void UpdateColor()
        {
            if (PART_Ellipse == null) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                Debug.WriteLine($"Updating color: IsOn={IsOn}, Fill={(IsOn ? OnColor : OffColor)}");
                PART_Ellipse.Fill = IsOn ? OnColor : OffColor;
            });
        }

    }
}
