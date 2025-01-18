using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks.Dataflow;
using Newtonsoft.Json;
using Scoreboard.Data;

namespace ScoreboardController.Data
{
    public interface IJsonMessenger
    {
        Task SendMessageAsync(object message);
        Task ReceiveMessagesAsync();
        Task ConnectAsync(string serverUrl);
    }

    public class JsonMessenger : IJsonMessenger
    {
        private readonly ClientWebSocket _webSocket;
        private readonly IMessageDispatcher _messenger;

        public JsonMessenger(IMessageDispatcher messenger)
        {
            _webSocket = new ClientWebSocket();
            _messenger = messenger;
        }

        public async Task ConnectAsync(string serverUrl)
        {
            await _webSocket.ConnectAsync(new Uri(serverUrl), CancellationToken.None);
            Console.WriteLine("Connected to WebSocket server.");
        }

        public async Task SendMessageAsync(object message)
        {
            string json = JsonConvert.SerializeObject(message);
            var buffer = Encoding.UTF8.GetBytes(json);
            await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            Console.WriteLine($"SENT: {json}");
        }

        public async Task ReceiveMessagesAsync()
        {
            var buffer = new byte[1024 * 4];
            while (_webSocket.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"RECEIVED: {message}");
                    var cmd = JsonConvert.DeserializeObject<CommandMessage>(message);
                    if (cmd != null) _messenger.DispatchMessage(cmd.Element, cmd.Value);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine("WebSocket closed.");
                    break;
                }
            }
        }

        public async Task CloseAsync()
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
        }
    }

    public class CommandMessage
    {
        public string Element { get; set; }
        public string Value { get; set; }
    }
}
