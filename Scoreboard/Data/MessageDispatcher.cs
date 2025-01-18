namespace Scoreboard.Data;

public class MessageDispatcher : IMessageDispatcher  
{
    private readonly Dictionary<string, IReceiveMessages> _elementRegistry;

    public MessageDispatcher()
    {
        _elementRegistry = new Dictionary<string, IReceiveMessages>();
    }

    public void RegisterElement(IReceiveMessages element)
    {
        if (!_elementRegistry.ContainsKey(element.ElementName))
        {
            _elementRegistry[element.ElementName] = element;
        }
    }

    public void DispatchMessage(string elementName, string value)
    {
        if (_elementRegistry.TryGetValue(elementName, out var element))
        {
            element.ReceiveMessage(value);
        }
        else
        {
            // Log or handle unknown element
            Console.WriteLine($"Element '{elementName}' not found.");
        }
    }
}