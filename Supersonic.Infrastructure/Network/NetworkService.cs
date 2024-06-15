using Microsoft.AspNetCore.Http;
using Supersonic.Core.Entities;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Supersonic.Infrastructure.Network
{
    public class NetworkService
    {
        private ClientWebSocket _webSocket;

        public async Task ConnectAsync(string url)
        {
            _webSocket = new ClientWebSocket();
            await _webSocket.ConnectAsync(new Uri(url), CancellationToken.None);
        }

        public async Task SendTransactionAsync(Transaction transaction, CancellationToken cancellationToken = default)
        {
            try
            {
                var transactionJson = JsonSerializer.Serialize(transaction);
                var transactionBytes = Encoding.UTF8.GetBytes(transactionJson);
                var segment = new ArraySegment<byte>(transactionBytes);
                await _webSocket.SendAsync(segment, WebSocketMessageType.Text, true, cancellationToken);
            }
            catch (Exception ex)
            {
                // Log or handle the error as needed
                Console.WriteLine($"Error sending transaction: {ex.Message}");
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
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var transaction = JsonSerializer.Deserialize<Transaction>(message);

                    if (transaction != null)
                    {
                        // Process the transaction
                    }

                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                }

                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, cancellationToken);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }
    }
}
