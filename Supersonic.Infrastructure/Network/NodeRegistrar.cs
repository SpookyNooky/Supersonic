namespace Supersonic.Infrastructure.Network
{
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class NodeRegistrar
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public NodeRegistrar(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task RegisterNodeAsync(string registryUrl, string nodeUrl)
        {
            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonSerializer.Serialize(nodeUrl), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{registryUrl}/api/registry/register", content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Node registered successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to register node. Status code: {response.StatusCode}");
            }
        }
    }
}
