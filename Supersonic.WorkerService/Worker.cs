using Microsoft.Extensions.Options;
using Supersonic.Core.Entities;
using Supersonic.Core.Interfaces;
using Supersonic.Core.Utils;
using Supersonic.Infrastructure.Network;
using Supersonic.WorkerService.Models;

namespace Supersonic.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConsensusMechanism _consensusMechanism;
        private readonly NetworkService _networkService;
        private readonly NodeConfiguration _nodeConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly NodeRegistrar _nodeRegistrar;

        public Worker(ILogger<Worker> logger, IConsensusMechanism consensusMechanism, NetworkService networkService, IOptions<NodeConfiguration> nodeConfigOptions, IHttpClientFactory httpClientFactory, NodeRegistrar nodeRegistrar)
        {
            _logger = logger;
            _consensusMechanism = consensusMechanism;
            _networkService = networkService;
            _nodeConfiguration = nodeConfigOptions.Value;
            _httpClientFactory = httpClientFactory;
            _nodeRegistrar = nodeRegistrar;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker service starting at: {time}", DateTimeOffset.Now);

            var nodeUrl = _nodeConfiguration.NodeUrl;

            // Register this node
            await _nodeRegistrar.RegisterNodeAsync(_nodeConfiguration.RegistryUrl, nodeUrl);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                // Generate key pair and create a transaction
                var (publicKey, privateKey, address) = KeyPairGenerator.GenerateKeyPair();
                var (publicKey2, privateKey2, address2) = KeyPairGenerator.GenerateKeyPair();

                var transaction = new Transaction
                {
                    Id = Guid.NewGuid().ToString(),
                    FromAddress = publicKey,
                    ToAddress = publicKey2,
                    Amount = 10
                };

                transaction.SignTransaction(privateKey);

                await Task.Delay(5000, stoppingToken);

                var nodes = await DiscoverNodesAsync();
                foreach (var node in nodes)
                {
                    if (node != nodeUrl)
                    {
                        await _networkService.ConnectAsync(node);
                        //await _networkService.SendTransactionAsync(transaction);
                    }
                }

                _logger.LogInformation("Transaction added and sent to network.");

                await Task.Delay(10000, stoppingToken); // Delay for demonstration
            }
        }

        private async Task<List<string>> DiscoverNodesAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var nodes = await client.GetFromJsonAsync<List<string>>($"{_nodeConfiguration.RegistryUrl}/api/registry/nodes");
                return nodes ?? new List<string>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error discovering nodes from registry");
                return new List<string>();
            }
        }
    }
}
