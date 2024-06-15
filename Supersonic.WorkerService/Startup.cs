using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Supersonic.Core.Interfaces;
using Supersonic.Infrastructure.Network;
using Supersonic.WorkerService.Models;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Supersonic.WorkerService
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<NodeConfiguration>(options =>
            {
                options.NodeUrl = Environment.GetEnvironmentVariable("NODE_URL") ?? "http://localhost:5001";
                options.RegistryUrl = Environment.GetEnvironmentVariable("REGISTRY_URL") ?? "http://localhost:5000";
            });

            services.AddSingleton<IConsensusMechanism, ConsensusMechanism>();
            services.AddHttpClient();
            services.AddTransient<NetworkService>();
            services.AddTransient<NodeRegistrar>();
            services.AddHostedService<Worker>();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebSockets();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                // Map the WebSocket endpoint
                endpoints.Map("/ws", WebSocketHandler);
            });
        }

        private async Task WebSocketHandler(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using (var webSocket = await context.WebSockets.AcceptWebSocketAsync())
                {
                    await HandleWebSocketConnection(context, webSocket);
                }
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        private async Task HandleWebSocketConnection(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
