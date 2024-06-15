using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using Supersonic.Core.Entities;

public class NetworkService
{
    private ClientWebSocket _webSocket;
    private readonly ILogger<NetworkService> _logger;

    public NetworkService(ILogger<NetworkService> logger)
    {
        _logger = logger;
    }

    public async Task ConnectAsync(string url)
    {
        try
        {
            _webSocket = new ClientWebSocket();
            await _webSocket.ConnectAsync(new Uri(url), CancellationToken.None);
            _logger.LogInformation("Connected to WebSocket server at {Url}", url);
        }
        catch (WebSocketException ex)
        {
            _logger.LogError(ex, "Error connecting to WebSocket server at {Url}", url);
            _webSocket.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error connecting to WebSocket server at {Url}", url);
            _webSocket.Dispose();
        }
    }

    public async Task SendTransactionAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        try
        {
            if (_webSocket.State != WebSocketState.Open)
            {
                _logger.LogWarning("WebSocket is not open. Current state: {State}", _webSocket.State);
                return;
            }

            var transactionJson = JsonSerializer.Serialize(transaction);
            var transactionBytes = Encoding.UTF8.GetBytes(transactionJson);
            var segment = new ArraySegment<byte>(transactionBytes);
            await _webSocket.SendAsync(segment, WebSocketMessageType.Text, true, cancellationToken);
            _logger.LogInformation("Transaction sent successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending transaction");
        }
    }

    public async Task ReceiveTransactionAsync(HttpContext context, CancellationToken cancellationToken = default)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

            while (!result.CloseStatus.HasValue)
            {
                try
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var transaction = JsonSerializer.Deserialize<Transaction>(message);

                    if (transaction != null)
                    {
                        // Process the transaction
                        _logger.LogInformation("Transaction received and processed.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error receiving transaction");
                }

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, cancellationToken);
        }
        else
        {
            context.Response.StatusCode = 400;
            _logger.LogWarning("Invalid WebSocket request.");
        }
    }
}
