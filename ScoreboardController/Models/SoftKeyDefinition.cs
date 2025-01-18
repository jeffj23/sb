using System.Windows.Input;

namespace ScoreboardController.Models;

public class SoftKeyDefinition
{
    public string Content { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public ICommand Command { get; set; } = null!;
}