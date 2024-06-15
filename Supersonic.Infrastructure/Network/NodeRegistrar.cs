using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Supersonic.Infrastructure.Network
{
    public class NodeRegistrar
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<NodeRegistrar> _logger;

        public NodeRegistrar(IHttpClientFactory httpClientFactory, ILogger<NodeRegistrar> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task RegisterNodeAsync(string registryUrl, string nodeUrl)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var content = new StringContent(JsonSerializer.Serialize(nodeUrl), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{registryUrl}/api/registry/register", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Node registered successfully at {RegistryUrl}", registryUrl);
                }
                else
                {
                    _logger.LogWarning("Failed to register node at {RegistryUrl}. Status code: {StatusCode}", registryUrl, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering node at {RegistryUrl}", registryUrl);
            }
        }
    }
}
