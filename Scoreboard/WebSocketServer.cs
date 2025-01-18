using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Scoreboard.Data;
using Scoreboard.Models;

public class WebSocketServer
{
    private readonly HttpListener _httpListener;
    private readonly IMessageDispatcher _dispatcher;

    public WebSocketServer(string url, IMessageDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add(url);
    }

    public async Task StartAsync()
    {
        _httpListener.Start();
        Console.WriteLine("WebSocket server started...");

        while (true)
        {
            var context = await _httpListener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                var webSocketContext = await context.AcceptWebSocketAsync(null);
                Console.WriteLine("WebSocket connected.");
                await HandleConnectionAsync(webSocketContext.WebSocket);
            }
            else
            {
                context.Response.StatusCode = 400; // Bad Request
                context.Response.Close();
            }
        }
    }

    private async Task HandleConnectionAsync(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"RECEIVED: {message}");
                var cmd = JsonConvert.DeserializeObject<CommandMessage>(message);
                _dispatcher.DispatchMessage(cmd.Element, cmd.Value);

                // Process the received message and optionally send a response
                var response = Encoding.UTF8.GetBytes("{\"status\":\"ok\"}");
                await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                Console.WriteLine("WebSocket closed.");
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
            }
        }
    }
}