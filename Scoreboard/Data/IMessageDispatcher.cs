namespace Scoreboard.Data;

public interface IMessageDispatcher
{
    public void RegisterElement(IReceiveMessages element);
    public void DispatchMessage(string elementName, string value);
}