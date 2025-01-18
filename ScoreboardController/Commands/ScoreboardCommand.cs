namespace ScoreboardController.Commands;

public class ScoreboardCommand
{
    public string ElementName { get; set; }
    public CommandType CommandType { get; set; }
    public string? Value { get; set; }
}