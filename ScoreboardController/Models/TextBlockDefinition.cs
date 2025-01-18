namespace ScoreboardController.Models;

public class TextBlockDefinition
{
    public string Name { get; set; }  // Unique identifier for the TextBlock
    public string? LabelForegroundColor { get; set; }
    public string ValueForegroundColor { get; set; }
    public string BackgroundColor { get; set; }
    public string BindingProperty { get; set; }  // Property to bind to
    public int FontSize { get; set; }
    public string FontWeight { get; set; }
    public string HorizontalAlignment { get; set; }
    public string VerticalAlignment { get; set; }
    public string TextAlignment { get; set; }
    public string PanelName { get; set; }
    public string? LabelText { get; set; }
    public string? DefaultText { get; set; }
    public int Row { get; set; }
    public int Col { get; set; }
    public int RowSpan { get; set; }
    public int ColSpan { get; set; }
    public string? LabelOrientation { get; set; }
}