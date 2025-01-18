namespace ScoreboardController.Commands
{
    public enum CommandType
    {
        None = 0,
        Start,
        Stop,
        Set,
        SetDefault,
        Reset,
        Increment,
        Decrement,
        Blank
    }
}